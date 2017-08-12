using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour {

	GameObject parentObject;

	void Start() {
		parentObject = this.gameObject.transform.parent.gameObject;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals("sword")) {
			parentObject.GetComponent<Enemy>().OnHit(other);
			//this.rb2d.AddForce(new Vector2(knockbackForce * scale, 100));
		}
	}
}
