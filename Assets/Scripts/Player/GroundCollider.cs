using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour {

	public PlayerController player;

	List<GameObject> currentCollisions = new List<GameObject>();

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.collider.tag.Contains("platform")) {
			player.HitGround(col);
			currentCollisions.Add(col.gameObject);
		}
	}

	void OnCollisionStay2D(Collision2D col) {
		if (col.collider.tag.Contains("platform")) {
			//player.StayOnGround(col);
		}
	}

	void OnCollisionExit2D(Collision2D col) {
		if (col.collider.tag.Contains("platform")){
			currentCollisions.Remove(col.gameObject);
		}
		if (currentCollisions.Count == 0) {
			player.LeaveGround(col);
		}
	}
}
