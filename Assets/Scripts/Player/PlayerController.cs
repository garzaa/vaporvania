using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity 
{

    [HideInInspector] public bool facingRight = false;
    [HideInInspector] public bool jump = false;
    public float jumpSpeed = 5f;
    public float moveSpeed = 5f;
    public float airControlRatio = .7f;

    public bool frozen;

	private bool grounded = false;
    private bool falling = true;
    private bool wallSliding = false;
    private bool touchingWall = false;

    private Animator anim;
    private Rigidbody2D rb2d;
	public GameObject sword;

	public bool swinging = false;

    //if this is true, the player is invincible to enemy attacks(?)
    private bool parrying;

    public bool attackCooldown = false;

	void Awake () 
	{
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sword.SetActive(false);
        swinging = false;
        grounded = false;
        anim.SetBool("falling", true);
    }

    void Update()
    {
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow)) && (grounded || wallSliding))
        {
            jump = true;
        }

    }

	void FixedUpdate () 
	{
		Attack();
        Move();
    }


	void OnCollisionEnter2D(Collision2D col)
	{
        //this is checked so they don't snap to idle after hitting their head on a ceiling
        //have different colliders in the future or something, because this is kinda stupid
        if (col.collider.tag == "platform" && col.transform.position.y < this.transform.position.y)
        {
            grounded = true;
            anim.SetBool("jumping", false);
            //cancel an aerial attack
            StopSwinging();
            //anim.SetBool("grounded", true);
            StopFalling();
            StopWallSliding();
        } else if (col.collider.tag.Contains("wall") && !grounded)
        {
            touchingWall = true;
            StopFalling();
            StartWallSliding();
        }
	}

    void OnCollisionStay2D(Collision2D col)
    {

        if (col.collider.tag == "platform" && col.transform.position.y < this.transform.position.y)
        {
            grounded = true;
            //anim.SetBool("grounded", true);
            anim.SetBool("jumping", false);
            StopFalling();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.tag == "platform" && col.transform.position.y < this.transform.position.y)
        {
            grounded = false;
            anim.SetBool("jumping", true);
        //else, if they're not jumping off a wall and instead just falling
        } else if (col.collider.tag.Contains("wall") && !Input.GetKey(KeyCode.UpArrow))
        {
            touchingWall = false;
            StopWallSliding();
            anim.SetBool("falling", true);
        }
    }

    void Jump()
    {
        if (grounded)
        {
            jump = true;
        }
    }

	void Attack()
	{
        if (attackCooldown) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Z) && !swinging && grounded && !Input.GetKey(KeyCode.DownArrow))
		{
            anim.SetTrigger("groundAttack");
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
			StartCoroutine (Swing ());
		} if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.DownArrow) && grounded)
        {
            Parry();
        } else {
            if (Input.GetKeyDown(KeyCode.Z) && !grounded && !swinging) {
                this.AirAttack();
            }
        }
	}

    void Parry() {
        anim.SetTrigger("parry");
    }

	IEnumerator Swing()
	{
		sword.SetActive (true);
		sword.transform.Translate (-.2f, 0, 0);
		yield return new WaitForSeconds(.05f);
		sword.transform.Translate (.2f, 0, 0);
		sword.SetActive (false);
		yield return new WaitForSeconds(.2f);
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
        frozen = true;
        //push up and away from the wall
        rb2d.velocity = new Vector2(-4 * (facingRight ? 1 : -1), 1.2f * jumpSpeed);
        yield return new WaitForSeconds(.1f);
        frozen = false;
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");

        //stop the player if they're moving on the ground
        if (Mathf.Abs(h) < 1 && grounded)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            anim.SetBool("running", false);
        }

        if (grounded && HorizontalInput() && !swinging && !frozen)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
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
        }

        if (!facingRight && rb2d.velocity.x > 0 && !Input.GetKey(KeyCode.LeftArrow))
        {
            Flip();
        }
        else if (facingRight && rb2d.velocity.x < 0 && !Input.GetKey(KeyCode.RightArrow))
        {
            Flip();
        }

        if (jump)
        {
            if (wallSliding)
            {
                //jump away from the current wall, and freeze player inputs so they don't get glued back
                StartCoroutine(WallJump());
            } else
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            }
            anim.SetTrigger("jump");
            jump = false;
        }

        //stop the sliding animation if needed
        if (wallSliding) {
            if (Mathf.Abs(rb2d.velocity.y) < 0.3) {
                anim.SetTrigger("wallstick");
            } else {
                anim.SetTrigger("wallunstick");
            }
        }
    }

    void StartFalling()
    {
        anim.SetBool("falling", true);
        falling = true;
    }

    void StopFalling()
    {
        anim.SetBool("falling", false);
        falling = false;
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

    void Flip() 
	{
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        //flip by scaling -1
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
}
