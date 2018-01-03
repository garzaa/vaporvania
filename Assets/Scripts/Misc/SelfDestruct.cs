using UnityEngine;

public class SelfDestruct : MonoBehaviour {

	public void Destroy() {
		Destroy(this.gameObject);
	}
}
