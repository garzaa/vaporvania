using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity 
{
    public int hp = 10;
    public int maxHP = 10;

    public float jumpSpeed = 5f;
    public float moveSpeed = 5f;
    public float airControlRatio = .7f;

	public bool grounded = false;
    private bool wallSliding = false;

    private Animator anim;
    public Rigidbody2D rb2d;

	public bool swinging = false;

    //if this is true, the player is invincible to enemy attacks and will trigger a riposte on hit
    //(todo)
    public bool parrying;

    public bool attackCooldown = false;

    public float ROLL_VELOCITY = -4f;
    public float DASH_SPEED = 20f;
    Vector2 preDashVelocity;
    bool fastFalling = false;

    List<KeyCode> forcedInputs;

    public int maxAirJumps = 0;
    private int airJumps;

    [HideInInspector] public GameObject platformTouching;

    GameObject currentHurtbox;
    public GameObject hurtboxes;

    public bool comboWindow;

    SpriteRenderer spr;
    Material defaultMaterial;
    Material cyanMaterial;
    Material redMaterial;
    int flashTimes = 5;

    public bool invincible = false;

    public bool inHitstop = false;

    public CameraShaker cameraShaker;

    public bool VAPOR_DASH = false;
    public bool DAMAGE_DASH = false;
    public bool dashing = false;
    public bool dashCooldown = false;

    //a few frames' window to parry (vapor+attack) when also pressing movement keys, since dash is (vapor+move)
    int dashTimeout = 0;
    int FRAME_WINDOW = 5;

    public bool savePossible = false;
    public GameController gc;

    //for triggering events/savepoints/etc
    private Collider2D playerTrigger;
    public GameObject savePoint;

	void Start () {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        swinging = false;
        grounded = false;
        anim.SetBool("falling", true);

        forcedInputs = new List<KeyCode>();

        airJumps = maxAirJumps;

        CloseAllHurtboxes();

        spr = this.GetComponent<SpriteRenderer>();
        defaultMaterial = spr.material;
        cyanMaterial = Resources.Load<Material>("Shaders/CyanFlash");
        redMaterial = Resources.Load<Material>("Shaders/RedFlash");
    }

	void FixedUpdate() {
        Jump();
        Move();
		Attack();
    }

    public void HitGround(Collision2D col) {
        InterruptAttack();
        if (col.transform.position.y < this.transform.position.y) {
            grounded = true;
            anim.SetBool("grounded", true);
            //cancel an aerial attack
            StopSwinging();
            //anim.SetBool("grounded", true);
            StopFalling();
            StopWallSliding();
            
            if (fastFalling) {
                if (HorizontalInput()) {
                    anim.SetTrigger("roll");
                } else {
                    anim.SetTrigger("hardLand");
                }
                createDust();
            }
        }
        this.airJumps = maxAirJumps;

        //track if they're on a passthrough, one-way platform
        platformTouching = col.collider.gameObject;
    }

    public void LeaveGround(Collision2D col) {
        grounded = false;
        anim.SetBool("grounded", false);
        if (!dashing) {
            if (rb2d.velocity.y < 0) {
              anim.SetTrigger("fall");
            }
        }
        platformTouching = null;
    }

    public void HitWall(Collision2D col) {
        StopFalling();
        StartWallSliding();
        InterruptAttack();
        this.airJumps = maxAirJumps;
    }

    public void LeaveWall(Collision2D col) {
        StopWallSliding();
        anim.SetBool("falling", true);
    }

    void Jump() {
        if (!(
            (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow)) && 
            (grounded || wallSliding || airJumps >= 0)
        ) || frozen) {
                return;
        }

        if (!grounded || !wallSliding) {
            airJumps--;
        }
        if (wallSliding)
        {
            //jump away from the current wall, and freeze player inputs so they don't instantly move back
            StartCoroutine(WallJump());
        } else
        {
            createDust();
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);

            //right now you can jump-cancel attacks and parries
            InterruptAttack();
        }
        anim.SetBool("grounded", false);
        anim.SetTrigger("jump");
        anim.SetBool("falling", false);
    }

	void Attack() {
        if (attackCooldown || parrying) {
            return;
        }
        //if dashTimeout > 0, that means the player has pressed Shift down in the last few frames
        if (Input.GetKeyDown(KeyCode.Z) && (Input.GetKey(KeyCode.LeftShift) || dashTimeout > 0) && !swinging && !dashing) {
            dashTimeout = 0;
            Parry();
        } 
        
        else if (Input.GetKeyDown(KeyCode.D) && !invincible) {
            Die();
        }

        else if (Input.GetKeyDown(KeyCode.Z) && CanGroundAttack()) {
            //in case of canceling another attack somehow
            //InterruptAttack();
            anim.SetTrigger("groundAttack");
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
		} 
        
        else if (Input.GetKeyDown(KeyCode.Z) && !grounded && !swinging && !wallSliding && !dashing) {
            AirAttack();
        }

        //can be generous with key checking here for reasons below
        else if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.DownArrow))
                || (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.DownArrow))) {
            Dodge();
        }

        //on pressing shift, wait a few frames to dash to give the player a window to press Z to parry
        //the dash timwout won't be started if the player has already started parrying, which is checked for at the start of this function
        else if (HorizontalInput() && Input.GetKeyDown(KeyCode.LeftShift)) {
            dashTimeout = FRAME_WINDOW;
        }
        
        //decrement the timer after pressing shift to dash, but leave a window for input combos
        if (dashTimeout > 0) {
            dashTimeout--;
            if (dashTimeout <= 0 && !frozen) {
                Dash();
            }
        }
	}

    void Parry() {
        InterruptAttack();
        anim.SetTrigger("parry");
    }

    /**
    This adds a small force opposite the wall and freezes the player for a moment so they don't 
    immediately move back to it.
     */
    IEnumerator WallJump()
    {
        anim.SetBool("wallSliding", false);
        this.wallSliding = false;
        anim.SetTrigger("jump");
        Freeze();
        //push up and away from the wall
        rb2d.velocity = new Vector2(-4 * (facingRight ? 1 : -1), 1.2f * jumpSpeed);
        yield return new WaitForSeconds(.1f);
        UnFreeze();
    }

    void Move() {
        float h = Input.GetAxis("Horizontal");

        //stop the player if they're moving on the ground
        //check if it's less than 1 because unity does weird smoothing on arrow key inputs
        if (Mathf.Abs(h) < 1 && grounded)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            anim.SetBool("running", false);
        }

        //interaction
        if (Input.GetKeyDown(KeyCode.X)) {
            if (savePossible && grounded) {
                gc.Save(this.savePoint);
            }
        }

        //check for no opposite inputs to prevent moonwalking
        if (grounded && HorizontalInput() && !swinging && !frozen)
        {
            if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
            }

            if ((rb2d.velocity.x != 0 || HorizontalInput()) && grounded)
            {
                anim.SetBool("running", true);
            }
        } else if (!grounded && HorizontalInput() && !swinging && !frozen)
        {
            //in the air, lessen control but maintain existing airspeed if moving at max
            if (Input.GetKey(KeyCode.RightArrow) && rb2d.velocity.x < moveSpeed)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x + moveSpeed * airControlRatio, rb2d.velocity.y);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && rb2d.velocity.x > (moveSpeed * -1))
            {
                rb2d.velocity = new Vector2(-moveSpeed * airControlRatio, rb2d.velocity.y);
            }
            
            //and then clamp the speed depending on movement direction
            if (rb2d.velocity.x < 0) {
                if (rb2d.velocity.x < -1 * moveSpeed) {
                    rb2d.velocity = new Vector2(-1 * moveSpeed, rb2d.velocity.y);
                }
            } else {
                if (rb2d.velocity.x > 1 * moveSpeed) {
                    rb2d.velocity = new Vector2(1 * moveSpeed, rb2d.velocity.y);
                }
            }
        }

        //flip sprites depending on movement direction
        if (!facingRight && rb2d.velocity.x > 0 && !Input.GetKey(KeyCode.LeftArrow))
        {
            Flip();
        }
        else if (facingRight && rb2d.velocity.x < 0 && !Input.GetKey(KeyCode.RightArrow))
        {
            Flip();
        }

        //stop the sliding animation if needed
        if (wallSliding) {
            if (Mathf.Abs(rb2d.velocity.y) < 0.2) {
                anim.SetTrigger("wallstick");
            } else {
                anim.SetTrigger("wallunstick");
            }
        }

        //if they press down to drop through a platform
        if (Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftShift) && !frozen) {
            //if they can actually drop through a platform
            if (platformTouching != null && platformTouching.GetComponent<PlatformEffector2D>() != null) {
                //disable the platform collider for a second
                platformTouching.GetComponent<PlatformController>().StartDisable();
                rb2d.AddForce(new Vector2(0, -100f));
                anim.SetTrigger("fall");
                anim.SetBool("grounded", false);
                this.grounded = false;
            }
        }

        if (dashing) {
            int moveScale = facingRight ? 1 : -1;
            rb2d.velocity = new Vector2(DASH_SPEED * moveScale, 0);
        }

        if (rb2d.velocity.y < ROLL_VELOCITY) {
            this.fastFalling = true;
        } else {
            this.fastFalling = false;
        }
    }

    void StartFalling()
    {
        anim.SetBool("falling", true);
    }

    void StopFalling()
    {
        anim.SetBool("falling", false);
    }

    void StopWallSliding()
    {
        anim.SetBool("wallSliding", false);
        anim.SetBool("falling", true);
        this.wallSliding = false;
    }

    void StartWallSliding()
    {
        anim.SetBool("wallSliding", true);
        anim.SetBool("falling", false);
        this.wallSliding = true;
    }
		
    bool HorizontalInput()
    {
        return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow);
    }

    void StartSwinging() {
        this.swinging = true;
    }

    void StopSwinging() {
        this.swinging = false;
    }

    public void AttackCooldown(float seconds) {
        this.comboWindow = false;
        StartCoroutine(StartAttackCooldown(seconds));
    }

    private IEnumerator StartAttackCooldown(float seconds) {
        this.attackCooldown = true;
        yield return new WaitForSeconds(seconds);
        this.attackCooldown = false;
    }

    void ResetAttackCooldown() {
        this.attackCooldown = false;
    }

    void AirAttack() {
        anim.SetTrigger("airAttack");
    }

    public void StartParrying() {
        this.parrying = true;
    }
    public void StopParrying() {
        this.parrying = false;
    }

    void StartForcing(KeyCode keyCode) 
    {
        //don't want to add duplicates
        if (!this.forcedInputs.Contains(keyCode)) {
            this.forcedInputs.Add(keyCode);
        }
    }
    void StopForcing(KeyCode keyCode) {
        this.forcedInputs.Remove(keyCode);
    }
    void StopForcingAll() {
        this.forcedInputs.Clear();
    }

    public void Freeze() {
        this.frozen = true;
    }
    public void UnFreeze() {
        this.frozen = false;
    }

    public void InterruptAttack() {
        //this should always be called
        this.CloseAllHurtboxes();
        this.CloseComboWindow();
        this.frozen = false;
        //InterruptDash();
        //right now you can jump cancel parries, but it could be a bit OP
        if (!swinging && !parrying) return;
        this.swinging = false;
        StopParrying();
        this.UnFreeze();
        this.attackCooldown = false;
    }

    void resetJumps() {
        airJumps = maxAirJumps;
    }

    //this stuff is to interface with animation events
    public void OpenHurtbox(string hurtboxName) {
        //find the jab1 object and activate it
        foreach (Transform hurtbox in hurtboxes.GetComponentInChildren<Transform>()) {
            if (hurtbox.name.Equals(hurtboxName)) {
                this.currentHurtbox = hurtbox.gameObject;
                hurtbox.GetComponent<BoxCollider2D>().enabled = true;
                return;
            }
        }
        Debug.LogError(hurtboxName + "not found");
    }

    public void CloseHurtbox(string hurtboxName) {
        if (this.currentHurtbox != null) {
            this.currentHurtbox.GetComponent<BoxCollider2D>().enabled = false;
        }
        this.currentHurtbox = null;
    }

    public void CloseAllHurtboxes() {
        foreach (Transform hurtbox in hurtboxes.GetComponentInChildren<Transform>()) {
            if (hurtbox.GetComponent<BoxCollider2D>().enabled) {
                hurtbox.GetComponent<BoxCollider2D>().enabled = false;
            } 
        }
    }

    public void OpenComboWindow() {
        this.comboWindow = true;
    }

    public void CloseComboWindow() {
        this.comboWindow = false;
    }

    private bool CanGroundAttack() {
        if (!grounded) return false;
        //fix the down arrow check, combine inputs more gracefully
        return (!swinging && !Input.GetKey(KeyCode.DownArrow)) || comboWindow;
    }

    public void CyanSprite() {
        spr.material = cyanMaterial;
    }

    public void RedSprite() {
        spr.material = redMaterial;
    }

    public void WhiteSprite() {
        spr.material = defaultMaterial;
    }

    IEnumerator Hurt(int flashes, bool first) {
        SetInvincible(true);
        if (first) {
            RedSprite();
            first = false;
        } else {
            CyanSprite();
        }
        yield return new WaitForSeconds(.07f);
        WhiteSprite();
        yield return new WaitForSeconds(.07f);
        if (flashes > 0) {
            StartCoroutine(Hurt(--flashes, first)); //;^)
        } else {
            SetInvincible(false);
        }
    }

    void StartHurting(int dmg) {
        this.hp -= dmg;
        if (this.hp <= 0) {
            Die();
            return;
        }
        StartCoroutine(Hurt(flashTimes, true));
    }

    public void OnMonsterHit(Collider2D boneHurtingCollider) {
        if (parrying) {
            //the special parry hitbox takes precedence here
            return;
        }

        if (invincible || inHitstop) return;

        int dmg;
        if (boneHurtingCollider.GetComponent<HurtboxController>() != null) {
            dmg = boneHurtingCollider.GetComponent<HurtboxController>().damage;
        } else {
            dmg = 1;
        }
        cameraShaker.SmallShake();
        this.StartHurting(dmg);
    }

    public void OnEnvDamage(Collider2D boneHurtingCollider) {
        if (invincible) return;
        int dmg = 1;
        if (boneHurtingCollider.GetComponent<HurtboxController>() != null) {
            dmg = boneHurtingCollider.GetComponent<HurtboxController>().damage;
        }
        this.StartHurting(dmg);
    }

    public void SetInvincible(bool b) {
        this.invincible = b;
    }

    public void Die() {
        this.Freeze();
        FreezeInSpace();
        SetInvincible(true);
        anim.SetBool("dead", true);
        anim.SetTrigger("die");
        StartCoroutine(WaitAndRespawn());
    }

    public IEnumerator WaitAndRespawn() {
        yield return new WaitForSeconds(1f);
        gc.Respawn();
    }

    public void Hide() {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Show() {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Dodge() {
        if (frozen || swinging || attackCooldown || !grounded) return;
        anim.SetTrigger("dodge");
    }

    //animation events :^)
    public void StartDodging() {
        this.SetInvincible(true);
        this.Freeze();
    }

    public void StopDodging() {        
        this.SetInvincible(false);
        this.UnFreeze();
        AttackCooldown(.2f);
    }

    //hhhgngh animation events don't support booleans
    public void MakeInvincible() {
        SetInvincible(true);
    }
    public void MakeVincible() {
        SetInvincible(false);
    }

    public void Riposte(GameObject enemyParent) {
        StopParrying();
        anim.SetTrigger("riposte");
        cameraShaker.SmallShake();
    }
    public void Dash() {
        if (dashCooldown || dashing || parrying) {
            return;
        }
        InterruptAttack();

        //damage dash always means invincibility
        if (VAPOR_DASH || DAMAGE_DASH) {
            CyanSprite();
            SetInvincible(true);
        }
        if (DAMAGE_DASH) {
            anim.SetTrigger("damageDash");
            OpenHurtbox("DamageDash");
        }
        anim.SetTrigger("dash");
        dashing = true;
        Freeze();

        //preserve horizontal velocity but cancel falling
        preDashVelocity = new Vector2(rb2d.velocity.x, 0);
    }

    public void StopDashing() {
        UnFreeze();
        dashing = false;
        rb2d.velocity = preDashVelocity;
        StartCoroutine(StartDashCooldown(.2f));
        if (VAPOR_DASH) {
            WhiteSprite();
            SetInvincible(false);
        }
        CloseHurtbox("DamageDash");
    }

    IEnumerator StartDashCooldown(float seconds) {
        dashCooldown = true;
        yield return new WaitForSeconds(seconds);
        dashCooldown = false;
    }

    void InterruptDash() {
        dashing = false;
        StartCoroutine(StartDashCooldown(.2f));
        if (VAPOR_DASH || DAMAGE_DASH) {
            WhiteSprite();
            SetInvincible(false);
        }
        CloseHurtbox("DamageDash");
    }

    public void GetHealth(int health) {
        if (this.hp < maxHP) {
            this.hp += health;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "savepoint") {
            other.GetComponent<Interactable>().AddPrompt();
            savePoint = other.gameObject;
			savePossible = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
        if (other.transform.IsChildOf(transform)) {
            return;
        }
		if (other.gameObject.tag == "savepoint") {
            other.GetComponent<Interactable>().RemovePrompt();
            savePoint = null;
			savePossible = false;
		}
	}

    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "savepoint") {
            other.GetComponent<Interactable>().AddPrompt();
            savePossible = true;
            savePoint = other.gameObject;
        }
    }

    public void FullHeal() {
        this.hp = maxHP;
    }

    public void Respawn() {
        UnFreeze();
        UnFreezeInSpace();
        SetInvincible(false);
        anim.SetBool("dead", false);
        anim.SetTrigger("respawn");
        FullHeal();
        Show();
    }
}