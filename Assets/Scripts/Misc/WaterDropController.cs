using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropController : SelfDestruct {

	Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
	}

	void OnCollisionEnter2D(Collision2D collision) {
		anim.SetTrigger("splash");
	}

}
