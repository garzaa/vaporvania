using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitmarkerController : MonoBehaviour {

	public int activeFrames = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (activeFrames-- <= 0) {
			Destroy(this.gameObject);
		}
	}
}
