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
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow)) && grounded)
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
        if (col.collider.tag == "platform" && col.transform.position.y < this.transform.position.y)
        {
            grounded = true;
            anim.SetBool("jumping", false);
            StopFalling();
        } else if (col.collider.tag.Contains("wall"))
        {
            StopFalling();
            anim.SetBool("wallSliding", true);
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
		if (Input.GetKeyDown(KeyCode.Z) && !swinging && grounded)
		{
            anim.SetTrigger("groundAttack");
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
			swinging = true;
			StartCoroutine (Swing ());
		}
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

    void Move()
    {
        float h = Input.GetAxis("Horizontal");

        //stop the player if they're moving on the ground
        if (Mathf.Abs(h) < 1 && grounded)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            anim.SetBool("running", false);
        }

        if (grounded && HorizontalInput() && !swinging)
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
        } else if (!grounded && HorizontalInput() && !swinging)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rb2d.velocity = new Vector2(moveSpeed * airControlRatio, rb2d.velocity.y);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb2d.velocity = new Vector2(-moveSpeed * airControlRatio, rb2d.velocity.y);
            }
        }

        if (!facingRight && rb2d.velocity.x > 0.1)
        { 
            Flip();
        }
        else if (facingRight && rb2d.velocity.x < -0.1)
        {
            Flip();
        }

        if (jump)
        {
            anim.SetBool("jumping", true);
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
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

    void Flip() 
	{
        Log("memememe");
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
