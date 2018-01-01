﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

	[HideInInspector] public Rigidbody2D rb2d;

	public int hp;
	public int moveSpeed;
	public int maxSpeed;

	public bool inHitstop;

	public float healthChance = 2f;
	public float moneyChance = 0f;

	public GameObject healthPrefab, moneyPrefab;

	[HideInInspector] public GameObject playerObject;

	[HideInInspector] public Animator anim;
	[HideInInspector] public bool hasAnimator;

	[HideInInspector] public EnemyBehavior[] behaviors;

	Material whiteMaterial;
	Material defaultMaterial;
	bool white;

	bool dead = false;

	public SpriteRenderer spr;

	// Use this for initialization
	void Start () {
		rb2d = this.GetComponent<Rigidbody2D>();
		playerObject = GameObject.Find("Player");
		if ((anim = this.GetComponent<Animator>()) != null) {
			this.hasAnimator = true;
		}
		behaviors = this.GetComponents<EnemyBehavior>();

		spr = this.GetComponent<SpriteRenderer>();
		defaultMaterial = spr.material;
		whiteMaterial = Resources.Load<Material>("Shaders/WhiteFlash");
	}

	public void DamageFor(int dmg) {
		this.hp -= dmg;
		if (this.hp <= 0 && !dead) {
			Die();
		}
	}

	public void OnHit(Collider2D other) {
		CheckDamage(other);
	}

	public void Die(){
		this.frozen = true;
		this.dead = true;
		DropPickups();
		CloseHurtboxes();
		if (this.GetComponent<Animator>() != null) {
			this.GetComponent<Animator>().SetTrigger("die");
		} else {
			Destroy();
		}
	}

	public void CheckDamage(Collider2D other) {
		//if it's a player sword
		if (other.tag.Equals("sword") || other.tag.Equals("playerAttack") && hasAnimator && !dead) {
			int scale = playerObject.GetComponent<PlayerController>().facingRight ? 1: -1;
			if (other.GetComponent<HurtboxController>() != null){
				this.rb2d.velocity = (new Vector2(other.GetComponent<HurtboxController>().knockbackVector.x * scale, other.GetComponent<HurtboxController>().knockbackVector.y));
			}

			anim.SetTrigger("hurt");
			DamageFor(other.gameObject.GetComponent<HurtboxController>().damage);
			WhiteSprite();
			white = true;

			Hitstop.Run(other.GetComponent<HurtboxController>().hitstop, this.gameObject);
		}

	}

	//for each added behavior, call it
	public void Update() {
		foreach (EnemyBehavior eb in this.behaviors) {
			eb.Move();
		}
		CheckFlip();
		if (white) {
			white = false;
			StartCoroutine(normalSprite());
		}
	}

	public void DropPickups() {
		DropHealth();
		DropMoney();
	}

	public void DropHealth() {
		if (Random.Range(0f, 1f) < healthChance) {
			GameObject h = (GameObject) Instantiate(healthPrefab, this.transform.position, Quaternion.identity);
			h.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(1, 3));
		}
	}

	//on death, remove damage dealing even though it'll live a little bit while the dying animation finishes
	public void CloseHurtboxes() {
		foreach (Transform child in transform) {
			if (child.gameObject.tag == "EnemyHitbox") {
				child.gameObject.SetActive(false);
			}
		}
	}

	public void DropMoney() {

	}

	public void WhiteSprite() {
        spr.material = whiteMaterial;
    }

	IEnumerator normalSprite() {
		yield return new WaitForSeconds(.05f);
		spr.material = defaultMaterial;
	}
}