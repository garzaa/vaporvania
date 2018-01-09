using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//extends SelfDestruct so the animation clip can call it
public class Breakable : SelfDestruct {

	Animator anim = null;
	GameObject wallCollider = null;
	bool broken = false;

	void Start() {
		if (GetComponent<Animator>() != null) {
			anim = GetComponent<Animator>();
		}
		foreach (Transform child in transform) {
			if (child.GetComponent<Collider2D>()) {
				wallCollider = child.gameObject;
				return;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == Tags.playerAttack && !broken) {
			Break();
		}
	}

	public void Break() {
		broken = true;
		if (wallCollider != null) {
			Destroy(wallCollider);
		}
		
		if (anim != null) {
			anim.SetTrigger("break");
		}
	}

}
