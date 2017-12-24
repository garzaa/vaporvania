using UnityEngine;

public class SavePointController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerController>().savePossible = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			other.gameObject.GetComponent<PlayerController>().savePossible = false;
		}
	}
}
