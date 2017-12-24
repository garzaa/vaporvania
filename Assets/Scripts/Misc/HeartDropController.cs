using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartDropController : MonoBehaviour {

	public int health = 1;
	public GameObject hitmarker;

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<PlayerController>().GetHealth(this.health);
			Instantiate(hitmarker, this.transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		}
	}
}
