using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SludgeballController : Enemy {

	// Use this for initialization
	void Start() {
		rb2d = this.GetComponent<Rigidbody2D>();
		playerObject = GameObject.Find("Player");
		anim = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	public void Move() {
		if (this.frozen) {
			return;
		}
		//move towards the player
		//first, get where they are
		if (Mathf.Abs(playerObject.transform.position.x - this.transform.position.x) < seekThreshold) {
			return;
		}
		int moveScale;
		if (playerObject.transform.position.x > this.transform.position.x) {
			moveScale = 1;
			this.movingRight = true;
		} else {
			moveScale = -1;
			this.movingRight = false;
		}

		if (Mathf.Abs(rb2d.velocity.x) < this.maxSpeed) {
			rb2d.AddForce(new Vector2(this.moveSpeed * moveScale, rb2d.velocity.y));
		}

		if (!facingRight && rb2d.velocity.x > 0 && movingRight)
        {
            Flip();
        }
        else if (facingRight && rb2d.velocity.x < 0 && !movingRight)
        {
            Flip();
        }
	}
}
