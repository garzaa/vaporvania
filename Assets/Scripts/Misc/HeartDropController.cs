using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartDropController : MonoBehaviour {

	public int health = 1;
	GameObject hitmarker;

	// Use this for initialization
	void Start () {
		hitmarker = (GameObject) Resources.Load("Prefabs/Particles/Hitmarker");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<PlayerController>().GetHealth(this.health);
			Instantiate(hitmarker, this.transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		}
	}
}
