using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHitboxController : MonoBehaviour {

	public PlayerController pc;

	BoxCollider2D parryHitbox;

	void Start() {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}

	public void OnTriggerEnter2D(Collider2D boneHurtingCollider) {
		if (pc.parrying && boneHurtingCollider.tag == Tags.enemyHitbox) {
			if (boneHurtingCollider.GetComponent<Projectile>()) {
                boneHurtingCollider.GetComponent<Projectile>().Reflect();
            } else if (boneHurtingCollider.transform.parent.GetComponent<Enemy>()) {
                if (boneHurtingCollider.tag == Tags.enemyHitbox) {
                    pc.Riposte(boneHurtingCollider.transform.parent.gameObject);
                }
            }
		}
	}
}
