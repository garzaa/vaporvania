using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public PlayerController pc;
	int lastHealth;
	Transform playerRespawnPoint;

	public Transform heartContainer;
	public Transform heartSprite;
	public Transform heartContainerSprite;

	GameObject savePoint;
	bool saving = false;

	int currentHearts;
	
	// Update is called once per frame
	void Update () {
		UpdateUI();

		//show the player once the save point animation is done
		if (this.savePoint != null) {

			if (savePoint.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("saving")) {
				saving = true;
			}

			//if saving has started, then an idle state means it's completed
			else if (saving && savePoint.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				pc.Show();
				pc.UnFreeze();
				pc.SetInvincible(false);
				//then unlink the save point and stop checking for saving
				this.savePoint = null;
				this.saving = false;
			}
		}
	}

	void InitUI() {

	}

	void UpdateUI() {
		UpdateHealth();
	}


	void UpdateHealth() {
		//only update UI if pc health changes
		if (currentHearts != pc.hp) {
			//clear all
			foreach(Transform child in heartContainer) {
    			Destroy(child.gameObject);
			}

			//then append to the heartContainer
			//offset: the distance sideways to put the next heart
			int offset = 0;
			for (int i=0; i<pc.hp; i++) {
				//create the first heart sprite
				Vector2 newpos = new Vector2(offset, 0);
				Transform currHeart = Instantiate(heartSprite, newpos, Quaternion.identity);
				currHeart.SetParent(heartContainer, worldPositionStays:false);

				//and then update the offset for the next heart image
				offset += 15;
				currentHearts = pc.hp;
			}

			//and then do the same for heart containers
			for (int j=pc.hp; j<pc.maxHP; j++) {
				Vector2 newpos = new Vector2(offset, 0);
				Transform currHeart = Instantiate(heartContainerSprite, newpos, Quaternion.identity);
				currHeart.SetParent(heartContainer, worldPositionStays:false);

				//and then update the offset for the next heart image
				offset += 15;
			}
		}
	}

	public void Save(GameObject sp) {
		//also need to find a way to freeze the player until the animation is finished
		//also hide them, but there's already a method for that
		this.savePoint = sp;
		pc.Hide();
		//then move the player character to the save point so it doesn't look weird
		pc.transform.position = new Vector3(sp.transform.position.x, pc.transform.position.y, pc.transform.position.z);
		//also, cancel their momentum
		pc.rb2d.velocity = Vector3.zero;
		//then lock further inputs
		pc.Freeze();
		pc.SetInvincible(true);
		//finally, heal
		pc.Heal();
		if (savePoint.GetComponent<Animator>() != null) {
			savePoint.GetComponent<Animator>().SetTrigger("save");
		}
	}
}
