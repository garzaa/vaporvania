using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour {

	GameObject parentObject;

	public GameObject hitmarker;

	void Start() {
		parentObject = this.gameObject.transform.parent.gameObject;
		hitmarker = (GameObject) Resources.Load("Prefabs/Particles/Hitmarker");
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag.Equals("playerAttack")) {
			parentObject.GetComponent<Enemy>().OnHit(other.gameObject.GetComponent<BoxCollider2D>());
			//instantiate a hitmarker at the point of contact
			//this works for tiny enemies, we might have to have multiple hitboxes on bosses (or SOMETHING else with dynamically calculating 
			//the collision midway point based on relative positions of the two hitboxes)
			Instantiate(hitmarker, this.transform.position, Quaternion.identity);

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
