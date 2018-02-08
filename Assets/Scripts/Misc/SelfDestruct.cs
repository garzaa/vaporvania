using System.Collections;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	public float timer = 0f;

	void Start() {
		if (timer > 0) {
			StartCoroutine(WaitAndDestroy(timer));
		}
	}

	public void Destroy() {
		Destroy(this.gameObject);
	}

	IEnumerator WaitAndDestroy(float time) {
		yield return new WaitForSeconds(time);
		Destroy();
	}
}
