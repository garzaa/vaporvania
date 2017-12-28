using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {

	public void StartDisable() {
		StartCoroutine(Disable());
	}

	IEnumerator Disable() {
		this.GetComponent<BoxCollider2D>().isTrigger = true;
		yield return new WaitForSeconds(0.3f); //disgusting, but works
		this.GetComponent<BoxCollider2D>().isTrigger = false;
	}
}
