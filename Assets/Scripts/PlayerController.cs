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
    public Transform groundCheck;


    public bool frozen;

	private bool grounded = false;
    private bool falling = true;
    private bool wallSliding = false;

    private Animator anim;
    private Rigidbody2D rb2d;
	public GameObject sword;

	private bool swinging = false;
    private bool parrying;

	void Awake () 
	{
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        sword.SetActive(false);
        swinging = false;
        grounded = false;
        anim.SetBool("falling", true);
    }

	void Start () 
	{
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
        //why is this checked...
        if (col.collider.tag == "platform" && col.transform.position.y < this.transform.position.y)
        {
            grounded = true;
            anim.SetBool("jumping", false);
            StopFalling();
            StopWallSliding();
        } else if (col.collider.tag.Contains("wall"))
        {
            StopFalling();
            StartWallSliding();
        }
	}

    void OnCollisionStay2D(Collision2D col)
    {

        if (col.collider.tag == "platform" && col.transform.position.y < this.transform.position.y)
        {
            grounded = true;
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
            StopWallSliding();
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
        if (Input.GetKeyDown(KeyCode.Z) && !swinging && grounded && !Input.GetKey(KeyCode.DownArrow))
		{
            anim.SetTrigger("groundAttack");
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
			swinging = true;
			StartCoroutine (Swing ());
		} if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.DownArrow) && grounded)
        {
            Parry();
        }
	}

    void Parry()
    {
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
		swinging = false;
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
        rb2d.velocity = new Vector2(-4 * (facingRight ? 1 : -1), 1.5f *jumpSpeed);
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
}
