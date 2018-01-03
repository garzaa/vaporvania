using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity 
{
    public int hp = 10;
    public int maxHP = 10;

    float jumpSpeed = 5f;
    float moveSpeed = 4f;
    float airControlRatio = .7f;

	public bool grounded = false;
    public bool wallSliding = false;

    private Animator anim;
    public Rigidbody2D rb2d;

	public bool swinging = false;

    //if this is true, the player is invincible to enemy attacks and will trigger a riposte on hit
    public bool parrying;

    public bool attackCooldown = false;

    float TERMINAL_VELOCITY = -10f;
    float ROLL_VELOCITY = -5f;
    float DASH_SPEED = 20f;
    Vector2 preDashVelocity;
    bool fastFalling = false;
    bool terminalFalling = false;

    List<KeyCode> forcedInputs;

    int maxAirJumps = 1;
    int airJumps;

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

    public bool savePossible = false;
    public GameController gc;

    //for triggering events/savepoints/etc
    private Collider2D playerTrigger;
    public GameObject savePoint;

    public bool animateSpawn = false;
    bool interactPossible = false;
    Interactable interactable;

    FightController fc;

    //for dialogue (also so it can be changed in-game?)
    public string playerName = "VAL";

    GameObject hitmarker;

    //analog jump, oboy
    //so the player jumps at 5f and it decreases until it hits zero
    //so then what should the cutoff be? should you be able to jump to 50% height?
    //let's say the cutoff is 3f
    //if the player's y-velocity is above this and the jump key is released, then set their y-velocity to this instead
    float JUMP_CUTOFF = 2f;

	void Start () {
        Flip();
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        fc = GetComponent<FightController>();
        hitmarker = (GameObject) Resources.Load("Prefabs/Particles/Hitmarker");

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

        if (savePoint != null) {
            this.transform.position = savePoint.transform.position;
            savePoint = null;
        }

        if (animateSpawn) {
            Respawn();
        }
        LeaveGround();
    }

	void FixedUpdate() {
		fc.Attack();
        Move();
        Jump();
    }

    public void HitGround(Collision2D col) {
        if (col.transform.position.y < this.transform.position.y) {
            InterruptAttack();
            this.grounded = true;
            anim.SetBool("grounded", true);
            //cancel an aerial attack
            swinging = false;
            StopFalling();
            StopWallSliding();
            
            if (fastFalling || terminalFalling) {
                if (HorizontalInput()) {
                    anim.SetTrigger("roll");
                } else {
                    anim.SetTrigger("hardLand");
                }
                CreateDust();

                if (terminalFalling) {
                    cameraShaker.SmallShake();
                }
            }
            
        }
        this.airJumps = maxAirJumps;

        //track if they're on a passthrough, one-way platform
        platformTouching = col.collider.gameObject;
    }

    public void StayOnGround(Collision2D col) {
        this.grounded = true;
        anim.SetBool("grounded", true);
    }

    public void LeaveGround() {
        grounded = false;
        anim.SetBool("grounded", false);
        if (!dashing && !Input.GetKey(KeyCode.UpArrow)) {
            anim.SetTrigger("fall");
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
            CreateDust();
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);

            //right now you can jump-cancel attacks and parries
            InterruptAttack();
        }
        anim.SetBool("grounded", false);
        anim.SetTrigger("jump");
        anim.SetBool("falling", false);
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
        if (Input.GetKeyDown(KeyCode.C) && grounded && !frozen && !dashing) {
            if (savePossible && !anim.GetCurrentAnimatorStateInfo(0).IsName("Spawn")) {
                gc.Save(this.savePoint);
            } else if (interactPossible) {
                this.interactable.Interact(this.gameObject);
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
        if (!facingRight && rb2d.velocity.x > 0 && !Input.GetKey(KeyCode.LeftArrow) && !frozen)
        {
            Flip();
        }
        else if (facingRight && rb2d.velocity.x < 0 && !Input.GetKey(KeyCode.RightArrow) && !frozen)
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

        if (rb2d.velocity.y < TERMINAL_VELOCITY) {
            this.terminalFalling = true;
            rb2d.velocity = new Vector2(rb2d.velocity.x, TERMINAL_VELOCITY);
        } else {
            this.terminalFalling = false;
        }

        //emulate an analog jump
        //if the jump button is released
        if (Input.GetKeyUp(KeyCode.UpArrow) && rb2d.velocity.y > JUMP_CUTOFF) {
            //then clamp the y velocity to the jump cutoff
            rb2d.velocity = new Vector2(rb2d.velocity.x, JUMP_CUTOFF);
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
        if (!grounded) anim.SetBool("falling", true);
        this.wallSliding = false;
    }

    void StartWallSliding()
    {
        anim.SetBool("wallSliding", true);
        anim.SetBool("falling", false);
        this.wallSliding = true;
    }
		
    public bool HorizontalInput()
    {
        return Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow);
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
        //InterruptDash();
        //right now you can jump cancel parries, but it could be a bit OP
        if (!swinging && !parrying) return;
        this.swinging = false;
        StopParrying();
        this.attackCooldown = false;
        anim.SetBool("running", false);
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

    public void CloseComboWindow() {
        this.comboWindow = false;
    }

    public bool CanGroundAttack() {
        if (!grounded){
            return false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Roll") || anim.GetCurrentAnimatorStateInfo(0).IsName("HardLand")) {
            return false;
        }
        //fix the down arrow check, combine inputs more gracefully
        return (!swinging && !Input.GetKey(KeyCode.DownArrow)) || (comboWindow);
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
        if (parrying || invincible || inHitstop) {
            return;
        }

        int dmg;
        if (boneHurtingCollider.GetComponent<HurtboxController>() != null) {
            dmg = boneHurtingCollider.GetComponent<HurtboxController>().damage;
        } else {
            dmg = 1;
        }
        Instantiate(hitmarker, this.transform.position, Quaternion.identity);
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
        InterruptAttack();
        InterruptDash();
        
        Freeze();
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

    public void Riposte(GameObject enemyParent) {
        enemyParent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
		if (other.gameObject.tag == "savepoint" && !frozen) {
            other.GetComponent<Interactable>().AddPrompt();
            savePoint = other.gameObject;
			savePossible = true;
		}
        else if (other.gameObject.tag == "interactable") {
            other.GetComponent<Interactable>().AddPrompt();
            this.interactable = other.GetComponent<Interactable>();
            this.interactPossible = true;
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
        else if (other.gameObject.tag == "interactable") {
            other.GetComponent<Interactable>().RemovePrompt();
            this.interactable = null;
            this.interactPossible = false;
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
        SetInvincible(false);
        anim.SetTrigger("respawn");
        
        FullHeal();
        
        //see what happens when you make your sprites all face left?
        if (!facingRight) {
            Flip();
        }
        Show();
    }

    public void SetLiveAnimation() {
        anim.SetBool("dead", false);
    }
}