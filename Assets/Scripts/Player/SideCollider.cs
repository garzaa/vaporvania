using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour {

	public PlayerController player;

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.collider.CompareTag(Tags.wall) || col.collider.CompareTag(Tags.platformwall)) {
			/* if (PositionCheck(this.GetComponent<Collider2D>(), col.gameObject.GetComponent<Collider2D>())) {
				player.HitWall(col);
			} /* else {
				//insert the potential ledge-climb animation here
			} */
			player.HitWall(col);
		}
		else if (col.collider.CompareTag(Tags.envdamage)) {
			player.OnEnvDamage(col.collider);
		}
		else if (col.collider.CompareTag(Tags.killzone)) {
			player.Die();
		}
	}

	void OnCollisionStay2D(Collision2D col) {
		if (col.collider.CompareTag(Tags.wall) || col.collider.CompareTag(Tags.platformwall)) {
			/* if (PositionCheck(this.GetComponent<Collider2D>(), col.gameObject.GetComponent<Collider2D>())) {
				player.StayOnWall(col);
			} else {
				player.LeaveWall(col);
			} */
			player.StayOnWall(col);
		}
	}

	void OnCollisionExit2D(Collision2D col) {
		if (col.collider.CompareTag(Tags.wall) || col.collider.CompareTag(Tags.platformwall))
			player.LeaveWall(col);
	}

	//make sure the upper bounds of the player collider are at or below the upper bounds of the wall collider
	bool PositionCheck(Collider2D player, Collider2D wall) {
		float playerMaxY = player.gameObject.transform.position.y + player.bounds.extents.y;
		float wallMaxY = wall.gameObject.transform.position.y + wall.bounds.extents.y;
		return playerMaxY <= wallMaxY;
	}
}
