using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateOnDeath : MonoBehaviour {

	public string gameState;
	public Entity trackedEntity;

	GameCheckpoints gc;

	void Start() {
		gc = GameObject.Find("GameController").GetComponent<GameCheckpoints>();
	}

	void Update() {
		if (trackedEntity == null) {
			gc.AddState(this.gameState);
			Destroy(this);
		}
	}
}
