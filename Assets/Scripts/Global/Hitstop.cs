using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitstop : MonoBehaviour{

	//statically calling instances
	public static Hitstop instance;

	void Awake() {
		instance = this;
	}

	public static void Run(float seconds, GameObject enemyParent) {
		instance.StartCoroutine(DoHitstop(seconds, enemyParent));
	}

	static IEnumerator DoHitstop(float seconds, GameObject enemyParent) {
		//pause animations for both entities
		bool frozePlayer = false;
		
		//store the last velocities
		Rigidbody2D rb2d = enemyParent.GetComponent<Rigidbody2D>();
		PlayerController pc = enemyParent.GetComponent<Enemy>().playerObject.GetComponent<PlayerController>();
		Vector2 lastV = rb2d.velocity;
		Vector2 lastPlayerV = pc.GetComponent<Rigidbody2D>().velocity;

		Animator parentAnim = enemyParent.GetComponent<Animator>();

		//freeze the positions, don't want to do it if the player is hitting multiple entities
		enemyParent.GetComponent<Enemy>().FreezeInSpace();
		if (!pc.inHitstop) {
			pc.FreezeInSpace();
			frozePlayer = true;
		}

		//freeze the animations
		if (parentAnim != null) {
			parentAnim.speed = 0;
		}
		pc.GetComponent<Animator>().speed = 0;

		yield return new WaitForSeconds(seconds);

		//then undo everything
		if (parentAnim != null) {
			parentAnim.speed = 1;
		}
		pc.GetComponent<Animator>().speed = 1;

		enemyParent.GetComponent<Enemy>().UnFreezeInSpace();
		if (frozePlayer) {
			pc.UnFreezeInSpace();
			frozePlayer = true;
		}

		pc.GetComponent<Rigidbody2D>().velocity = lastPlayerV;
		rb2d.velocity = lastV;
	}
}
