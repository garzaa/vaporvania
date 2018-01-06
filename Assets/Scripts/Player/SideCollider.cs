using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour {

	public PlayerController player;

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.collider.tag.Contains("wall")) {
			player.HitWall(col);
		}
		else if (col.collider.tag.Equals("killzone")) {
			player.Die();
		}
	}

	void OnCollisionExit2D(Collision2D col) {
		if (col.collider.tag.Contains("wall"))
			player.LeaveWall(col);
	}
}
