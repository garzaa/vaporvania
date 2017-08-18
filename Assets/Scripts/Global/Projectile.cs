using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public GameObject parent;
	public GameObject target;

	public Collider2D hitbox;
	public Rigidbody2D rb2d;

	Vector2 startingVector;

	void Start() {
		this.hitbox = GetComponent<Collider2D>();
		this.rb2d = GetComponent<Rigidbody2D>();

		if (startingVector != null) {
			rb2d.velocity = startingVector;
		}
	}

	public void Reflect() {
		Instantiate(Resources.Load("Prefabs/Hitmarker"), new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
		GameObject temp = this.target;
		this.target = this.parent;
		this.parent = temp;
		rb2d.velocity = new Vector2(rb2d.velocity.x * -1, rb2d.velocity.y * -1);
	}
}
