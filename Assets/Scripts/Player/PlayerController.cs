using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity 
{
    public int hp = 10;
    public int maxHP = 10;

    public int BASE_ATTACK_DMG = 1;

    readonly float jumpSpeed = 5f;
    readonly float moveSpeed = 3.5f;
    private float airControlRatio = .7f;

	public bool grounded = false;
    public bool wallSliding = false;

    private Animator anim;
    public Rigidbody2D rb2d;

	public bool swinging = false;

    //if this is true, the player is invincible to enemy attacks and will trigger a riposte on hit
    public bool parrying;

    public bool attackCooldown = false;

    [HideInInspector] public readonly float TERMINAL_VELOCITY = -8f;
    readonly float ROLL_VELOCITY = -5f;
    readonly float DASH_SPEED = 15f;
    Vector2 preDashVelocity;
    bool fastFalling = false;
    bool terminalFalling = false;

    public int maxAirJumps = 1;
    int airJumps;

    [HideInInspector] public GameObject platformTouching;

    GameObject currentHurtbox;
    public GameObject hurtboxes;

    public bool comboWindow;

    SpriteRenderer spr;
    Material defaultMaterial;
    Material cyanMaterial;
    int flashTimes = 5;

    public bool invincible = false;

    public bool inHitstop = false;

    public CameraShaker cameraShaker;

    public bool DAMAGE_DASH = true;
    public bool dashing = false;
    public bool dashCooldown = false;
    public bool dashReversal = false;

    public bool savePossible = false;
    public GameController gc;
    UIController uc;

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

    float JUMP_CUTOFF = 2.0f;

    //sounds
    public AudioSource jumpSound;
    public AudioSource pickupSound;

	void Start () {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        fc = GetComponent<FightController>();
        uc = gc.GetComponent<UIController>();
        hitmarker = (GameObject) Resources.Load("Prefabs/Particles/Hitmarker");

        swinging = false;
        grounded = false;
        anim.SetBool("falling", true);

        airJumps = maxAirJumps;

        CloseAllHurtboxes();

        spr = this.GetComponent<SpriteRenderer>();
        defaultMaterial = spr.material;
        cyanMaterial = Resources.Load<Material>("Shaders/CyanFlash");

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
            //cancel all of this if there's a meteor attack happening
            if (fc.inMeteor) {
                    fc.LandMeteorBlade(col);
            } 
            else if (fastFalling || terminalFalling) {
                if (terminalFalling) {
                    cameraShaker.SmallShake();
                }
                CreateDust();
                if (HorizontalInput()) {
                    anim.SetTrigger("roll");
                } else {
                    anim.SetTrigger("hardLand");
                }
                CreateDust();
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
        if ((!dashing && !Input.GetButton("Jump")) 
            //passing upwards through a platform
            || rb2d.velocity.y > 0) {
            anim.SetTrigger("fall");
        }
        platformTouching = null;
        if (wallSliding) {
            StartWallSliding();
        }
    }

    public void HitWall(Collision2D col) {
        StopFalling();
        StartWallSliding();
        InterruptAttack();
        anim.SetBool("running", false);
        this.airJumps = maxAirJumps;
    }

    public void LeaveWall(Collision2D col) {
        StopWallSliding();
        anim.SetBool("falling", true);
    }

    public void StayOnWall(Collision2D col) {
        anim.SetBool("running", false);
        this.airJumps = maxAirJumps;
        this.wallSliding = true;
        InterruptAttack();
        StopFalling();
    }

    void Jump() {
        if (!(
            (Input.GetButtonDown("Jump")) && 
            (grounded || wallSliding || airJumps > 0)
        ) || frozen) {
                return;
        }

        jumpSound.Play();

        if (wallSliding && !grounded)
        {
            //jump away from the current wall, and freeze player inputs so they don't instantly move back
            StartCoroutine(WallJump());
            anim.SetBool("grounded", false);
            anim.SetTrigger("jump");
            anim.SetBool("falling", false);
        } 
        
        else if (!wallSliding && !grounded) {
            airJumps--;
            anim.SetBool("grounded", false);
            anim.SetTrigger("airJump");
            anim.SetBool("falling", false);
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed * 1.1f);
            CreateDust();
            InterruptAttack();
        } 
        
        else {
            anim.SetBool("grounded", false);
            anim.SetTrigger("jump");
            anim.SetBool("falling", false);
            CreateDust();
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);

            //right now you can jump-cancel attacks and parries
            InterruptAttack();
        }
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
        rb2d.velocity = new Vector2(-4 * GetForwardScalar(), 1.2f * jumpSpeed);
        yield return new WaitForSeconds(.1f);
        UnFreeze();
    }

    void Move() {
        if (Input.GetKeyDown(KeyCode.K)) {
            Die();
        }

        float h = Input.GetAxis("Horizontal");

        //stop the player if they're moving on the ground
        //check if it's less than 1 because unity does weird smoothing on arrow key inputs
        if ((Mathf.Abs(h) < 1 && grounded) || frozen)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            anim.SetBool("running", false);
        }

        //crouching
        if (!(Input.GetKey(KeyCode.DownArrow)) && anim.GetBool("crouchInput")) {
            anim.SetBool("crouchInput", false);
        } else if (Input.GetKeyDown(KeyCode.DownArrow) && !frozen) {
            anim.SetBool("crouchInput", true);
        }

        //interaction
        if (Input.GetKeyDown(KeyCode.C) && grounded && !frozen && !dashing) {
            if (savePossible && !anim.GetCurrentAnimatorStateInfo(0).IsName("Spawn")) {
                gc.Save(this.savePoint);
            } else if (interactPossible) {
                this.interactable.Interact(this.gameObject);
            }
        }

        //decrease airspeed when not moving in the air or jumping off walls
        if (!grounded && !frozen && !HorizontalInput()) {
            rb2d.velocity = new Vector2(rb2d.velocity.x * 0.9f, rb2d.velocity.y);
        }

        //dash reversal - uncomment to allow a single one
        if (dashReversal) {
            if (Input.GetKeyDown(KeyCode.RightArrow) && !facingRight) {
                Flip();
                //dashReversal = false;
            } else if (Input.GetKeyDown(KeyCode.LeftArrow) && facingRight) {
                Flip();
                //dashReversal = false;
            }
        }

        //check for no opposite inputs to prevent moonwalking
        if (HorizontalInput()) {
            if (grounded && !swinging && !frozen)
            {
                if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
                {
                    rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
                }
                else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
                {
                    rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
                }

                //again, prevent moonwalking
                if (rb2d.velocity.x != 0 && HorizontalInput() && grounded)
                {
                    anim.SetBool("running", true);
                }
            } 
            else if (!grounded && !frozen)
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
            if (rb2d.velocity.y > -0.2) {
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
            int moveScale = GetForwardScalar();
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
        if (Input.GetButtonUp("Jump") && rb2d.velocity.y > JUMP_CUTOFF) {
            //then decrease the y velocity to the jump cutoff
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
        fc.inMeteor = false;
        fc.StopMeteorBlade();
    }

    void ResetJumps() {
        airJumps = maxAirJumps;
    }

    public void CloseHurtbox(string hurtboxName) {
        if (this.currentHurtbox != null) {
            this.currentHurtbox.GetComponent<Collider2D>().enabled = false;
        }
        this.currentHurtbox = null;
    }

    public void CloseAllHurtboxes() {
        foreach (Transform hurtbox in hurtboxes.GetComponentInChildren<Transform>()) {
            if (hurtbox.GetComponent<Collider2D>().enabled) {
                hurtbox.GetComponent<Collider2D>().enabled = false;
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
        return (!swinging || comboWindow);
    }

    public void CyanSprite() {
        //in case the material was changed between start and now
        defaultMaterial = spr.material;
        spr.material = cyanMaterial;
    }

    public void WhiteSprite() {
        spr.material = defaultMaterial;
    }

    IEnumerator Hurt(int flashes) {
        SetInvincible(true);
        CyanSprite();
        yield return new WaitForSeconds(.07f);
        WhiteSprite();
        yield return new WaitForSeconds(.07f);
        if (flashes > 0) {
            StartCoroutine(Hurt(--flashes)); //;^)
        } else {
            SetInvincible(false);
        }
    }

    void StartHurting(int dmg) {
        this.hp -= dmg;
        //show an alert if low health
        if (this.hp <= 1 && this.hp > 0){
            uc.DisplayAlert(new Alert("WARNING: CORE CRITICAL", true));
        }

        if (this.hp <= 0) {
            uc.DisplayAlert(new Alert("FATAL ERROR: CORE DESTABILIZED", true));
            Die();
            return;
        }
        StartCoroutine(Hurt(flashTimes));
    }

    public void OnMonsterHit(Collider2D boneHurtingCollider) {
        if (parrying || invincible || inHitstop) {
            return;
        }

        int dmg;
        if (boneHurtingCollider.GetComponent<HurtboxController>() != null) {
            dmg = boneHurtingCollider.GetComponent<HurtboxController>().GetDamage();
        } else {
            dmg = 1;
        }
        Instantiate(hitmarker, this.transform.position, Quaternion.identity);
        cameraShaker.SmallShake();
        this.StartHurting(dmg);
    }

    public void OnEnvDamage(Collider2D boneHurtingCollider) {
        if (fc.inMeteor) {
            fc.StopMeteorBlade();
            anim.SetTrigger("hurt");
        }
        cameraShaker.SmallShake();
        if (invincible) return;
        int dmg = 1;
        if (boneHurtingCollider.GetComponent<HurtboxController>() != null) {
            dmg = boneHurtingCollider.GetComponent<HurtboxController>().GetDamage();
        }
        this.StartHurting(dmg);
    }

    public void SetInvincible(bool b) {
        this.invincible = b;
    }

    public void Die() {
        this.hp = 0;
        InterruptAttack();
        InterruptDash();
        
        Freeze();
        FreezeInSpace();
        SetInvincible(true);
        anim.ResetTrigger("groundAttack");
        anim.ResetTrigger("respawn");
        anim.SetBool("dead", true);
        anim.SetTrigger("die");
    }

    public void RespawnFromAnimation() {
        gc.Respawn();
    }

    public void Hide() {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Show() {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
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
        if (enemyParent.GetComponent<Rigidbody2D>() != null) {
            enemyParent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        StopParrying();
        anim.SetTrigger("riposte");
        cameraShaker.SmallShake();
    }

    public void Dash() {
        if (dashCooldown || dashing || parrying) {
            return;
        }

        fc.PlaySwing();
        InterruptAttack();

        SetInvincible(true);

        if (DAMAGE_DASH) {
            anim.SetTrigger("damageDash");
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
        if (DAMAGE_DASH) { 
            WhiteSprite();
        }
        SetInvincible(false);
        CloseHurtbox("DamageDash");
    }

    IEnumerator StartDashCooldown(float seconds) {
        dashCooldown = true;
        yield return new WaitForSeconds(seconds);
        dashCooldown = false;
    }

    public void InterruptDash() {
        dashing = false;
        StartCoroutine(StartDashCooldown(.2f));
        if (DAMAGE_DASH) {
            WhiteSprite();
        }
        SetInvincible(false);
        CloseHurtbox("DamageDash");
    }

    public void GetHealth(int health) {
        if (this.hp < maxHP) {
            this.hp += health;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == Tags.savepoint && !frozen) {
            other.GetComponent<Interactable>().AddPrompt();
            savePoint = other.gameObject;
			savePossible = true;
		}
        else if (other.gameObject.tag == Tags.interactable) {
            other.GetComponent<Interactable>().AddPrompt();
            this.interactable = other.GetComponent<Interactable>();
            this.interactPossible = true;
        }
	}

	void OnTriggerExit2D(Collider2D other) {
        if (other.transform.IsChildOf(transform)) {
            return;
        }
		if (other.gameObject.tag == Tags.savepoint) {
            other.GetComponent<Interactable>().RemovePrompt();
            savePoint = null;
			savePossible = false;
		}
        else if (other.gameObject.tag == Tags.interactable) {
            other.GetComponent<Interactable>().RemovePrompt();
            this.interactable = null;
            this.interactPossible = false;
        }
	}

    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == Tags.savepoint) {
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

    public void ZeroVelocity() {
        rb2d.velocity = Vector2.zero;
    }

    public void ClearInteractables() {
        this.savePoint = null;
        this.savePossible= false;
        this.interactable = null;
        this.interactPossible = false;
    }
    
    public Vector2 GetBottomCenter() {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
        return new Vector2(bc.transform.position.x, bc.bounds.min.y);
    }

    public void PlayPickup() {
        pickupSound.Play();
    }
}