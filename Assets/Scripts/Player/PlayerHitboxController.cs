using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxController : MonoBehaviour {

	public PlayerController pc;

	void Start() {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}

	void OnTriggerEnter2D(Collider2D boneHurtingCollider) {
		if (boneHurtingCollider.gameObject.CompareTag(Tags.enemyHurtbox)) {
			pc.OnMonsterHit(boneHurtingCollider);
		} else if(boneHurtingCollider.gameObject.tag.Equals("envDamage")) {
			pc.OnEnvDamage(boneHurtingCollider);
		}
	}

	void OnTriggerStay2D(Collider2D boneHurtingCollider) {
		if (!pc.invincible) {
			OnTriggerEnter2D(boneHurtingCollider);
		}
	}
}
