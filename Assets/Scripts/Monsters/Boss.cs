using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy {

	Vector2 playerPos;

	public string bossName;

	Transform hurtboxes;

	//always keep track of the player
	void UpdatePlayerPos() {
		this.playerPos = playerObject.transform.position;
	}

	new void Update() {
		UpdatePlayerPos();
		BossMove();
	}

	public virtual void BossMove() {

	}

}
