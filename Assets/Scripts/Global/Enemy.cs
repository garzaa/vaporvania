using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

	public Rigidbody2D rb2d;

	public int hp;
	public int moveSpeed;
	public int maxSpeed;

	public float seekThreshold = .2f;

	// Use this for initialization
	void Start () {
		rb2d = this.GetComponent<Rigidbody2D>();
	}

	void Hurt() {
		this.hp--;
		if (this.hp <= 0) {
			Destroy(this.gameObject);
		}
	}
}
