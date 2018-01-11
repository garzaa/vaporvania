using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitmarkerController : MonoBehaviour {

	public int activeFrames = 3;

	// Use this for initialization
	void Start () {
		
	}

	void Destroy() {
		Destroy(this.gameObject);
	}
}
