using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity 
{
    public float speed;
	public float jumpForce;
    public float maxSpeed;
    public float jumpHeight; //add analogue-esque jump later
    public bool facingRight = false;
    public bool frozen;

	private bool grounded;

    private Animator anim;
	private Rigidbody2D rb;
	public GameObject sword;
	private bool swinging;

	void Awake () 
	{
	}

	void Start () 
	{
        speed = 0.1f;
        anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		jumpForce = 200.0f;
		sword.SetActive (false);
		swinging = false;
	}

	void FixedUpdate () 
	{
		Move ();
		Attack ();
	}


	void OnCollisionEnter2D(Collision2D col)
	{
        if (col.collider.tag == "platform")
        {
            grounded = true;
            anim.SetBool("jumping", false);
        }
	}

	void Attack()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !swinging)
		{
			swinging = true;
			StartCoroutine (Swing ());
		}
	}

	IEnumerator Swing()
	{
		sword.SetActive (true);
		sword.transform.Translate (-.4f, 0, 0);
		yield return new WaitForSeconds(.05f);
		sword.transform.Translate (.4f, 0, 0);
		sword.SetActive (false);
		yield return new WaitForSeconds(.5f);
		swinging = false;
	}

	void Move () {

		/* Jump. */
		if (grounded) 
		{
            //can't hold jump to jump continuously
			if (Input.GetKeyDown(KeyCode.UpArrow)) 
			{
				grounded = false;
				rb.AddForce (new Vector2 (0, jumpForce));
                anim.SetBool("running", false);
                if (!anim.GetBool("jumping")) {
                    anim.SetBool("jumping", true);
                }
			}
		}
			
		/* Run left. */
		if (Input.GetKey(KeyCode.LeftArrow)) 
		{
            if (grounded) {
                anim.SetBool("running", true);
            }
			transform.Translate(-speed, 0, 0);
			if (facingRight && !frozen) 
				Flip();
		} 

		/* Run right. */
		else if (Input.GetKey(KeyCode.RightArrow)) 
		{
            if (grounded)
            {
                anim.SetBool("running", true);
            }
            transform.Translate(speed, 0, 0);
			if (!facingRight && !frozen) 
				Flip();
		} 

		/* Stand still. */
		else 
			anim.SetBool("running", false);
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
