using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : Entity {

	[HideInInspector] public Rigidbody2D rb2d;

	public int hp;
	public int totalHP;
	public int moveSpeed;
	public int maxSpeed;

	[HideInInspector] public bool inHitstop;

	public float healthChance = 2f;
	public float moneyChance = 0f;

	public GameObject healthPrefab, moneyPrefab;

	public GameObject playerObject;

	[HideInInspector] public Animator anim;
	[HideInInspector] public bool hasAnimator;

	[HideInInspector] public EnemyBehavior[] behaviors;

	Material whiteMaterial;
	Material defaultMaterial;
	bool white;

	bool dead = false;
	[HideInInspector] public bool invincible = false;

	public bool staggerable = true;
	public bool envDmgSusceptible = true;

	public SpriteRenderer spr;

	void OnEnable() {
		totalHP = hp;
		rb2d = this.GetComponent<Rigidbody2D>();
		playerObject = GameObject.Find("Player");
		if ((anim = this.GetComponent<Animator>()) != null) {
			this.hasAnimator = true;
		}
		behaviors = this.GetComponents<EnemyBehavior>();

		spr = this.GetComponent<SpriteRenderer>();
		defaultMaterial = spr.material;
		whiteMaterial = Resources.Load<Material>("Shaders/WhiteFlash");
		Initialize();
	}

	public virtual void Initialize() {

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
		CloseHurtboxes();
		this.frozen = true;
		this.dead = true;
		DropPickups();
		if (this.GetComponent<Animator>() != null) {
			this.GetComponent<Animator>().SetTrigger("die");
		} else {
			Destroy();
		}
	}

	public void CheckDamage(Collider2D other) {
		//if it's a player sword
		if (other.CompareTag(Tags.sword) || other.CompareTag(Tags.playerAttack) && !dead) {
			if (!invincible) {
				if (staggerable) {

					if (hasAnimator) {
						anim.SetTrigger("hurt");
					}

					int scale = playerObject.GetComponent<PlayerController>().GetForwardScalar();
					playerObject.GetComponent<FightController>().AttackConnect(this.gameObject);
					if (other.GetComponent<HurtboxController>() != null && this.rb2d != null) {
						this.rb2d.velocity = (new Vector2(other.GetComponent<HurtboxController>().knockbackVector.x * scale, 
							other.GetComponent<HurtboxController>().knockbackVector.y));
					}
				}

				DamageFor(other.gameObject.GetComponent<HurtboxController>().GetDamage());
				WhiteSprite();
				white = true;
			}

			Hitstop.Run(other.GetComponent<HurtboxController>().hitstop, this.gameObject);
			OnDamage();
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
		ExtendedUpdate();
	}

	public virtual void ExtendedUpdate() {

	}

	public void DropPickups() {
		DropHealth();
		DropMoney();
	}

	public void DropHealth() {
		if (Random.Range(0f, 1f) < healthChance) {
			GameObject h = (GameObject) Instantiate(healthPrefab, this.transform.position, Quaternion.identity);
			h.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(3, 5));
		}
	}

	//on death, remove damage dealing even though it'll live a little bit while the dying animation finishes
	public void CloseHurtboxes() {
		foreach (Transform child in transform) {
			if (child.gameObject.tag.Equals(Tags.enemyHurtbox)) {
				child.GetComponent<Collider2D>().enabled = false;
			}
		}
	}

	public void DropMoney() {
		if (Random.Range(0f, 1f) < moneyChance) {
			GameObject m = (GameObject) Instantiate(moneyPrefab, this.transform.position, Quaternion.identity);
			m.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-1, 1), Random.Range(3, 5));
		}
	}

	public void WhiteSprite() {
		defaultMaterial = spr.material;
        spr.material = whiteMaterial;
    }

	IEnumerator normalSprite() {
		yield return new WaitForSeconds(.05f);
		spr.material = defaultMaterial;
	}

	public virtual void OnDamage() {

	}
}