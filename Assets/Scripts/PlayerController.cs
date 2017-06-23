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

    private Animator anim;
	private CharacterController cc;
	private Vector3 moveDirection = Vector3.zero;

	void Awake () 
	{
	}

	void Start () 
	{
        speed = 0.1f;
        anim = GetComponent<Animator> ();
		cc = GetComponent<CharacterController> ();
		jumpSpeed = 5.0f;
		gravity = 10.0f;
	}

	void FixedUpdate () 
	{
		Move ();
	}

	void Move () {

		/* Jump. */
		if (cc.isGrounded) 
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			if (Input.GetKey(KeyCode.UpArrow))
				moveDirection.y = jumpSpeed;
		}
		moveDirection.y -= gravity * Time.deltaTime;
		cc.Move(moveDirection * Time.deltaTime);
		
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
