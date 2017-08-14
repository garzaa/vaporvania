using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

	public Rigidbody2D rb2d;

	public int hp;
	public int moveSpeed;
	public int maxSpeed;

	public float seekThreshold = .2f;

	public float knockbackSpeed = 3;

	public GameObject playerObject;

	public Animator anim;
	public bool hasAnimator;

	// Use this for initialization
	void Start () {
		rb2d = this.GetComponent<Rigidbody2D>();
		playerObject = GameObject.Find("Player");
		if ((anim = this.GetComponent<Animator>()) != null) {
			this.hasAnimator = true;
		}
	}

	public void DamageFor(int dmg) {
		this.hp -= dmg;
		if (this.hp <= 0) {
			Die();
		}
	}

	public void OnHit(Collider2D other) {
		CheckDamage(other);
	}

	public void Die(){
		this.frozen = true;
		if (this.GetComponent<Animator>() != null) {
			this.GetComponent<Animator>().SetTrigger("die");
		} else {
			Destroy();
		}
	}

	public void CheckDamage(Collider2D other) {
		//if it's a player sword
		if (other.tag.Equals("sword")) {
			int scale = playerObject.GetComponent<PlayerController>().facingRight ? 1: -1;
			this.rb2d.velocity = (new Vector2(knockbackSpeed * scale, 1));
		}
		if (hasAnimator) {
			anim.SetTrigger("hurt");
		}
		DamageFor(other.gameObject.GetComponent<HurtboxController>().damage);
	}
}
