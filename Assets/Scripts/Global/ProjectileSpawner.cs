using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour {

	int i;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (i++ % 60 == 0) {
			Instantiate(Resources.Load("Prefabs/Projecetiles/Test"), new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
		}
	}
}
