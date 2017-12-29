using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTeleporter : Interactable {

	public string sourceName;
	public string destScene;
	public string destName;

	TransitionController tc;
	GameController gc;

	void Start() {
		tc = GameObject.Find("GameController").GetComponent<TransitionController>();
		gc = GameObject.Find("GameController").GetComponent<GameController>();
	}

	public override void Interact(GameObject player) {
		player.GetComponent<PlayerController>().Freeze();
		player.GetComponent<PlayerController>().SetInvincible(true);
		gc.teleportTarget = destName;
		tc.LoadSceneFade(destScene);
	}
}
