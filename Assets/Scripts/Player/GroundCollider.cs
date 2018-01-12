using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour {

	public PlayerController player;

	void OnCollisionEnter2D(Collision2D col)
	{	
		//the collider needs to be lower so the player doesn't get grounded when they're jumping up through a platform
		if (col.collider.tag.Contains("platform") 
		&& col.gameObject.GetComponent<Collider2D>().bounds.max.y <= this.GetComponent<Collider2D>().transform.position.y) {
			player.HitGround(col);
		} else if (col.gameObject.CompareTag(Tags.killzone) && player.hp > 0) {
			player.cameraShaker.SmallShake();
			player.Die();
		}
	}

	void OnCollisionStay2D(Collision2D col) {
		if (col.collider.tag.Contains("platform") && col.gameObject.GetComponent<Collider2D>().bounds.max.y <= this.GetComponent<Collider2D>().bounds.min.y) {
			player.StayOnGround(col);
		}
	}

	void OnCollisionExit2D(Collision2D col) {
		if (col.collider.tag.Contains("platform")){
			player.LeaveGround();
		}
	}
}
