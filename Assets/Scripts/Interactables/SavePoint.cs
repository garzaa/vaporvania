using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : Interactable {

	public override void Interact(GameObject playerObject) {
		Object.FindObjectOfType<GameController>().Save(this.gameObject);
	}
}
