using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTeleporter : Interactable {

	public string sourceName;
	public string destScene;
	public string destName;

	TransitionController tc;

	void Start() {
		tc = GameObject.Find("GameController").GetComponent<TransitionController>();
	}

	public override void Interact(GameObject player) {
		player.GetComponent<PlayerController>().Freeze();
		player.GetComponent<PlayerController>().SetInvincible(true);

		tc.LoadSceneFade(destScene);
	}
}
