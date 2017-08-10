using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour {

	public PlayerController player;

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.collider.tag.Contains("platform"))
			player.HitGround(col);
	}

	void OnCollisionStay2D(Collision2D col) {
		if (col.collider.tag.Contains("platform"))
			player.StayOnGround(col);
	}

	void OnCollisionExit2D(Collision2D col) {
		if (col.collider.tag.Contains("platform"))
			player.LeaveGround(col);
	}
}
