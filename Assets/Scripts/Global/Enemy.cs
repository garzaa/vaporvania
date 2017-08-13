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

	public void Damage(int dmg) {
		this.hp -= dmg;
		if (this.hp <= 0) {
			Die();
		}
	}

	public virtual void OnHit(Collider2D other) {}

	public void Die(){
		this.frozen = true;
		if (this.GetComponent<Animator>() != null) {
			this.GetComponent<Animator>().SetTrigger("die");
		} else {
			Destroy();
		}
	}
}
