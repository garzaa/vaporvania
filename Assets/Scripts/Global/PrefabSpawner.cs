using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour {

	public float interval = .2f;

	public GameObject toSpawn;

	void Start() {
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn() {
		yield return new WaitForSeconds(interval);
		Instantiate(toSpawn, this.transform.position, Quaternion.identity);
		StartCoroutine(Spawn());
	}
}
