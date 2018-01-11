using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour {

	public GameObject parentObject;

	PlayerController pc;
	void Start() {
		pc = parentObject.GetComponent<Enemy>().playerObject.GetComponent<PlayerController>();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.playerAttack)) {
			parentObject.GetComponent<Enemy>().OnHit(other.gameObject.GetComponent<Collider2D>());
			HurtboxController hurtbox = other.GetComponent<HurtboxController>();
			GameObject hitmarker = hurtbox.hitmarker;
			//instantiate a hitmarker at the point of contact
			//this works for tiny enemies, we might have to have multiple hitboxes on bosses (or SOMETHING else with dynamically calculating 
			//the collision midway point based on relative positions of the two hitboxes)
			GameObject h = (GameObject) Instantiate(hitmarker, this.transform.position, Quaternion.identity);
			SpriteRenderer spr = h.GetComponent<SpriteRenderer>();
			if (hurtbox.flipHitmarker) {
				spr.flipX = !spr.flipX;
			}
			//then flip again
			if (!pc.facingRight) {
				spr.flipX = !spr.flipX;
			}

			//check for camera shake
			HurtboxController otherHurtbox;
			if ((otherHurtbox = other.gameObject.GetComponent<HurtboxController>()) != null) {
				if (otherHurtbox.cameraShake) {
					//this will always be the active camera according to Unity engine rules, so as long as it has a shaker we're good
					GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<CameraShaker>().SmallShake();
				}
			}
		}
	}

}
