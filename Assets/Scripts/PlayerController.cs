using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity 
{

    [HideInInspector] public bool facingRight = false;
    [HideInInspector] public bool jump = false;
    public float jumpSpeed = 100f;
    public float moveSpeed = 100f;
    public Transform groundCheck;


    public bool frozen;

	private bool grounded = false;

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
    }

	void Start () 
	{
	}

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

	void FixedUpdate () 
	{
		Attack();
        float h = Input.GetAxis("Horizontal");

        //stop the player if they're moving on the ground
        if (Mathf.Abs(h) < 1 && grounded)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            anim.SetBool("running", false);
        }

        if (grounded)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
            }

            if (rb2d.velocity.x != 0 && grounded)
            {
                anim.SetBool("running", true);
            }
        }

        if (rb2d.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (rb2d.velocity.x < 0 && facingRight)
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


	void OnCollisionEnter2D(Collision2D col)
	{
        if (col.collider.tag == "platform" && col.transform.position.y < this.transform.position.y)
        {
            grounded = true;
            anim.SetBool("jumping", false);
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
		if (Input.GetKeyDown(KeyCode.Z) && !swinging)
		{
            anim.SetTrigger("groundAttack");
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
        
    }

    void Flip() 
	{
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        //flip by scaling -1
    }
		
}
