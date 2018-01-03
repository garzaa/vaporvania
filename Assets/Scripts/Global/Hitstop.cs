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
		Vector2 lastPlayerV = pc.GetComponent<Rigidbody2D>().velocity;

		Animator parentAnim = enemyParent.GetComponent<Animator>();

		//freeze the positions, don't want to do it if the player is hitting multiple entities
		enemyParent.GetComponent<Enemy>().inHitstop = true;
		if (!pc.inHitstop) {
			pc.FreezeInSpace();
			frozePlayer = true;
			pc.inHitstop = true;
		}

		//freeze the animations
		if (parentAnim != null) {
			parentAnim.speed = 0;
		}
		pc.GetComponent<Animator>().speed = 0;
		Vector2 lastV = rb2d.velocity;

		//don't want to unfreeze the enemy afterwards if they're already frozen for some reason
		//so if they're not already frozen, then freeze them and store that info
		bool frozenEnemy = false;
		if (!enemyParent.GetComponent<Enemy>().frozenInSpace) {
			enemyParent.GetComponent<Enemy>().FreezeInSpace();
			frozenEnemy = true;
		}
		yield return new WaitForSeconds(seconds);

		//then undo everything
		if (parentAnim != null) {
			parentAnim.speed = 1;
		}
		pc.GetComponent<Animator>().speed = 1;

		//the enemy might have died
		if (enemyParent != null) {
			//also then unfreeze them if they were frozen from hitstop
			if (frozenEnemy) {
				enemyParent.GetComponent<Enemy>().UnFreezeInSpace();
			}
			rb2d.velocity = lastV;
			enemyParent.GetComponent<Enemy>().inHitstop = false;
		}

		if (frozePlayer) {
			pc.GetComponent<Rigidbody2D>().velocity = lastPlayerV;
			pc.inHitstop = false;
			pc.UnFreezeInSpace();
		}
	}
}
