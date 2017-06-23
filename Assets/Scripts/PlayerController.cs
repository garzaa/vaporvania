using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {

    public float speed;
    public float maxSpeed;
    public float jumpHeight; //add analogue-esque jump later
    public bool facingRight = false;
    public bool frozen;

    private Animator anim;



	// Use this for initialization
	void Start () {
        speed = .08f;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.LeftArrow))
        {
            anim.SetBool("running", true);
            transform.Translate(-speed, 0, 0);
            if (facingRight && !frozen)
            {
                Flip();
            }
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            anim.SetBool("running", true);
            transform.Translate(speed, 0, 0);
            if (!facingRight && !frozen)
            {
                Flip();
            }
        } else
        {
            anim.SetBool("running", false);
        }
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
