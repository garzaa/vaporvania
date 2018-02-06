using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartDropController : MonoBehaviour {

	public int health = 1;
	public GameObject hitmarker;

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "Player") {
			PlayerController pc = col.gameObject.GetComponent<PlayerController>();
			pc.GetHealth(this.health);
			pc.PlayPickup();
			Instantiate(hitmarker, this.transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		} else if (col.gameObject.CompareTag(Tags.envdamage)) {
			Destroy(this.gameObject);
		}
	}
}
