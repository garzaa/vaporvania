using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour {

	public GameObject hitmarker;

	public Boss b;

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag.Equals("playerAttack")) {
			b.OnHit(other.gameObject.GetComponent<Collider2D>());
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
