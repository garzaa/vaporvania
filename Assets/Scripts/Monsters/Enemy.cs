using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

	[HideInInspector] public Rigidbody2D rb2d;

	public int hp;
	public int moveSpeed;
	public int maxSpeed;

	public bool inHitstop;

	public float seekThreshold = .2f;

	[HideInInspector] public GameObject playerObject;

	[HideInInspector] public Animator anim;
	[HideInInspector] public bool hasAnimator;

	[HideInInspector] public EnemyBehavior[] behaviors;

	// Use this for initialization
	void Start () {
		rb2d = this.GetComponent<Rigidbody2D>();
		playerObject = GameObject.Find("Player");
		if ((anim = this.GetComponent<Animator>()) != null) {
			this.hasAnimator = true;
		}
		behaviors = this.GetComponents<EnemyBehavior>();
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
		if (other.tag.Equals("sword") || other.tag.Equals("playerAttack")) {
			int scale = playerObject.GetComponent<PlayerController>().facingRight ? 1: -1;
			this.rb2d.velocity = (new Vector2(other.GetComponent<HurtboxController>().knockbackVector.x * scale, other.GetComponent<HurtboxController>().knockbackVector.y));
			Hitstop.Run(other.GetComponent<HurtboxController>().hitstop, this.gameObject);
		}
		if (hasAnimator) {
			anim.SetTrigger("hurt");
		}
		DamageFor(other.gameObject.GetComponent<HurtboxController>().damage);
	}

	//for each added behavior, call it
	public void Update() {
		foreach (EnemyBehavior eb in this.behaviors) {
			eb.Move();
		}
		CheckFlip();
	}
}
