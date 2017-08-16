using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekPlayerY : EnemyBehavior {

	public override void Move() {
		if (mainController.frozen || playerDistance > maxSeekThreshold) {
			return;
		}
		//move towards the player
		//first, get where they are
		if (Mathf.Abs(playerObject.transform.position.y - this.transform.position.y) < minSeekThreshold) {
			return;
		}
		int moveScale;
		if (playerObject.transform.position.y > this.transform.position.y) {
			moveScale = 1;
			mainController.movingRight = true;
		} else {
			moveScale = -1;
			mainController.movingRight = false;
		}

		if (Mathf.Abs(mainController.rb2d.velocity.y) < mainController.maxSpeed) {
			mainController.rb2d.AddForce(new Vector2(mainController.rb2d.velocity.x, (mainController.moveSpeed * moveScale)));
		}
	}
}
