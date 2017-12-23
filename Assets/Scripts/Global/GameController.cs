using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	public PlayerController pc;
	int lastHealth;

	public Transform heartContainer;
	public Transform heartSprite;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		UpdateUI();
	}

	void InitUI() {

	}

	void UpdateUI() {
		UpdateHealth();
	}

	void UpdateHealth() {
		//only update UI if pc health changes
		if (heartContainer.childCount != pc.hp) {
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
				offset += 30;
			}
		}
	}
}
