using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity 
{
    public float speed;
	public float jumpSpeed;
    public float maxSpeed;
    public float jumpHeight; //add analogue-esque jump later
    public bool facingRight = false;
    public bool frozen;
	private float gravity;

	private bool grounded;

    private Animator anim;
	private Vector3 moveDirection = Vector3.zero;
	private BoxCollider bc;

	void Awake () 
	{
	}

	void Start () 
	{
        speed = 0.1f;
        anim = GetComponent<Animator> ();
		bc = GetComponent<BoxCollider> ();
		jumpSpeed = 5.0f;
		gravity = 10.0f;
	}

	void FixedUpdate () 
	{
		Move ();
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.collider.tag == "platform")
			grounded = true;
	}

	void Move () {

		/* Jump. */
		if (grounded) 
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetKey (KeyCode.UpArrow)) 
			{
				grounded = false;
				moveDirection.y = jumpSpeed;
			}
		}
		moveDirection.y -= gravity * Time.deltaTime;
		transform.Translate(moveDirection * Time.deltaTime);
		
		/* Run left. */
		if (Input.GetKey(KeyCode.LeftArrow)) 
		{
			anim.SetBool("running", true);
			transform.Translate(-speed, 0, 0);
			if (facingRight && !frozen) 
				Flip();
		} 

		/* Run right. */
		else if (Input.GetKey(KeyCode.RightArrow)) 
		{
			anim.SetBool("running", true);
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
