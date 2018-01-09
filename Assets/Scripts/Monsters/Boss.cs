using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
So the boss needs to:
1. detect when the player walks into the boss fight area
2. freeze the player, maybe play an animation depending on whether it's been fought before
3. open dialogue, maybe?
4. start the boss fight, which involves enclosing the area (maybe call the walls' animation trigger to lower them)
5. transition between phases
6. maybe do things on a function of health? this can all be done in BossMove()
7. die
8. reward the player
9. update game state accordingly
 */


public class Boss : Enemy {

	Vector2 playerPos;

	public string bossName;
	public bool foughtBefore = false;

	Transform hurtboxes;

	BoxCollider2D activationTrigger;

	bool fighting = false;

	//always keep track of the player
	void UpdatePlayerPos() {
		this.playerPos = playerObject.transform.position;
	}

	new void Update() {
		UpdatePlayerPos();
		BossMove();
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.Player)) {
			StartFight();
		}
	}

	public virtual void BossMove() {

	}

	public virtual void StartFight() {
		
	}

}
