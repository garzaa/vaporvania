using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : Interactable {


	public string text;

	UIController uc;

	void Start() {
		uc = GameObject.Find("GameController").GetComponent<UIController>();
	}

	public override void Interact(GameObject player) {
		print("opening dialog");
		uc.OpenDialogue(this);

		uc.RenderText(text);
	}
}
