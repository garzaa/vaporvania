using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour {

	public float interval = .2f;
	public bool randomStart;

	public GameObject toSpawn;

	void Start() {
		StartCoroutine(StartSpawning());
	}

	IEnumerator StartSpawning() {
		if (randomStart) {
			yield return new WaitForSeconds(Random.Range(0, 2));
		}
		StartCoroutine(Spawn());
	}

	IEnumerator Spawn() {
		yield return new WaitForSeconds(interval);
		GameObject tmp = (GameObject) Instantiate(toSpawn, this.transform.position, Quaternion.identity);
		tmp.transform.parent = this.transform;
		StartCoroutine(Spawn());
	}
}
