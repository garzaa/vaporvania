using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour {

	GameObject parentObject;

	public GameObject hitmarker;

	void Start() {
		parentObject = this.gameObject.transform.parent.gameObject;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag.Equals("playerAttack")) {
			parentObject.GetComponent<Enemy>().OnHit(other.gameObject.GetComponent<BoxCollider2D>());
			Hitstop.Run(.04f, parentObject);
			//instantiate a hitmarker at the point of contact
			//this works for tiny enemies, we might have to have multiple hitboxes on bosses (or SOMETHING else with dynamically calculating 
			//the collision midway point based on relative positions of the two hitboxes)
			Instantiate(Resources.Load("Prefabs/Hitmarker"), new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
		}
	}

}
