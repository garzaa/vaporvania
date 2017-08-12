using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SludgeballController : Enemy {

	//behavior: follows the player
	GameObject playerObject;

	// Use this for initialization
	void Start() {
		rb2d = this.GetComponent<Rigidbody2D>();
		playerObject = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs(playerObject.transform.position.x - this.transform.position.x) < seekThreshold) {
			return;
		}

		int moveScale = playerObject.transform.position.x > this.transform.position.x ? 1 :-1;

		if (Mathf.Abs(rb2d.velocity.x) < this.maxSpeed) {
			rb2d.AddForce(new Vector2(this.moveSpeed * moveScale, rb2d.velocity.y));
		}

		if (!facingRight && rb2d.velocity.x > 0)
        {
            Flip();
        }
        else if (facingRight && rb2d.velocity.x < 0)
        {
            Flip();
        }
	}
}
