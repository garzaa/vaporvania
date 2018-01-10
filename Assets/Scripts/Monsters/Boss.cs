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

so then the script will rely on:
a ui controller to freeze the player and open dialogue
a collider2d trigger to detect when the player enters the area
some walls to lower? i guess it depends on the boss, maybe
	the equivalent could just be disabling exits or whatever
a game controller to store the final state
 */

public class Boss : Enemy {

	Vector2 playerPos;

	public string bossName;
	public bool foughtBefore = false;

	Transform hurtboxes;

	BoxCollider2D activationTrigger;

	public bool fighting = false;

	public Sprite[] bossPortraits;
	public UIController uc;

	public GameController gc;

	public List<DialogueLine> monologue;
	int currentLine = 0;

	//always keep track of the player
	void UpdatePlayerPos() {
		this.playerPos = playerObject.transform.position;
	}

	public override void ExtendedUpdate() {
		if (!fighting) return;
		UpdatePlayerPos();
		BossMove();
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.Player) && !fighting) {
			StartFight();
		}
	}

	public virtual void BossMove() {

	}

	public virtual void StartFight() {

	}

	public void AdvanceLine() {
		if (++currentLine == monologue.Count) {
			currentLine = 0;
			uc.CloseDialogue();
		} else {
			uc.RenderDialogue(monologue[currentLine]);
		}
	}

	//called when the intro dialogue is closed
    public virtual void StopTalking() {
        
    }
}
