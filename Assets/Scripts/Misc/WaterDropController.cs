using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropController : SelfDestruct {

	const float termminalY = -12f;

	Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		anim.SetTrigger("splash");
	}

	//clamp velocity since they're falling from high ceilings
	void Update() {
		Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
		if (rb2d.velocity.y < termminalY) {
			rb2d.velocity = new Vector2(rb2d.velocity.x, termminalY);
		}
	}

}
