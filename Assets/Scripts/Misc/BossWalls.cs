using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWalls : MonoBehaviour {

	public Animator anim;

	void Start() {
		this.anim = GetComponent<Animator>();
	}

	public void LowerWalls() {
		anim.SetTrigger("lower");
	}

	public void RaiseWalls() {
		anim.SetTrigger("raise");
	}
}
