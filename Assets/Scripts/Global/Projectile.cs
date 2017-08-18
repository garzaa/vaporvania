using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity {

	[HideInInspector] public GameObject parent;
	[HideInInspector] public GameObject target;

	[HideInInspector] public Collider2D hitbox;
	[HideInInspector] public Rigidbody2D rb2d;
	public bool reflectable = true;

	public bool piercing;
	public Vector2 startingVector;

	void Start() {
		this.hitbox = GetComponent<Collider2D>();
		this.rb2d = GetComponent<Rigidbody2D>();
		//null checks are apparently always true for Vector2s
		if (startingVector.magnitude != 0) {
			rb2d.velocity = startingVector;
		}
	}

	public void Reflect() {
		if (!reflectable) return;
		this.reflectable = false;
		Instantiate(Resources.Load("Prefabs/Hitmarker"), new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
		//change the hitbox to a player-friendly version
		this.gameObject.tag = "playerAttack";
		GameObject temp = this.target;
		this.target = this.parent;
		this.parent = temp;
		rb2d.velocity = new Vector2(rb2d.velocity.x * -1, rb2d.velocity.y * -1);
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (!piercing) Destroy(this.gameObject);
	}

	public void Update() {
		CheckFlip();
	}

}
