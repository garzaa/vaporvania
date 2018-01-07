﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	GameController gc;
	PlayerController pc;

	public Transform healthContainer;
	public Transform heartSprite;
	public Transform heartContainerSprite;

	int currentHearts;

	//dialogue
	NPC currentNPC;	
	//these are to be hooked up in the editor along with heart containers as above
	public Image dialogueBox;
	public Text dialogueText;
	public Image currentPortrait;
	public Image advanceArrow;
	public Sprite playerPortrait;
	public Text speakerName;

	//signs
	Sign currentSign;
	public Sprite signPortrait;

	bool openedThisFrame = false;

	void Start() {
		gc = GetComponent<GameController>();
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
		HideDialogueUI();
		ClearText();
		ClearPortrait();
	}

	void Update() {
		UpdateUI();
		CheckForLineAdvance();
		//don't immediately advance dialogue on the first opening
		if (openedThisFrame) {
			openedThisFrame = false;
		}
	}

	void UpdateUI() {
		UpdateHealth();
	}

	void CheckForLineAdvance() {
		if (openedThisFrame) {
			openedThisFrame = false;
			return;
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			if (currentNPC != null) {
				currentNPC.AdvanceLine();
			} else if (currentSign != null) {
				CloseDialogue();
			}
		}
	}

	void UpdateHealth() {
		//only update UI if pc health changes
		if (currentHearts != pc.hp) {
			//clear all
			foreach(Transform child in healthContainer) {
    			Destroy(child.gameObject);
			}

			//then append to the heartContainer
			//offset: the distance sideways to put the next heart
			int offset = 0;
			for (int i=0; i<pc.hp; i++) {
				//create the first heart sprite
				Vector2 newpos = new Vector2(offset, 0);
				Transform currHeart = Instantiate(heartSprite, newpos, Quaternion.identity);
				currHeart.SetParent(healthContainer, worldPositionStays:false);

				//and then update the offset for the next heart image
				offset += 15;
				currentHearts = pc.hp;
			}

			//and then do the same for heart containers
			for (int j=pc.hp; j<pc.maxHP; j++) {
				Vector2 newpos = new Vector2(offset, 0);
				Transform currHeart = Instantiate(heartContainerSprite, newpos, Quaternion.identity);
				currHeart.SetParent(healthContainer, worldPositionStays:false);

				//and then update the offset for the next heart image
				offset += 15;
			}
		}
	}

	public void OpenDialogue(NPC npc) {
		openedThisFrame = true;
		this.currentNPC = npc;
		pc.Freeze();
		pc.SetInvincible(true);
		SetPortrait(npc.portraits[0]);
		ShowDialogueUI();
	}

	public void OpenDialogue(Sign sign) {
		openedThisFrame = true;
		currentSign = sign;
		if (sign.portrait != null) {
			SetPortrait(sign.portrait);
		} else {
			SetPortrait(signPortrait);
		}

		if (!string.IsNullOrEmpty(sign.signName)) {
			SetName(sign.signName);
		}

		pc.Freeze();
		pc.SetInvincible(true);
		ShowDialogueUI();
	}

	//called by the NPC controller if the NPC is out of dialogue
	public void CloseDialogue() {
		pc.UnFreeze();
		pc.SetInvincible(false);
		this.currentNPC = null;
		this.currentSign = null;
		HideDialogueUI();
	}

	//for one-off signs
	public void RenderText(string text) {
		SetText(text);
	}

	//also called by the NPC controller, there's some intermediary parsing that goes on here
	public void RenderDialogue(DialogueLine line) {
		//setting the player portrait for a reply
		if (line.image < 0) {
			SetPortrait(playerPortrait);
			SetName(pc.playerName);
		} else {
			SetPortrait(currentNPC.portraits[line.image]);
			SetName(line.name);
		}
		SetText(line.text);
	}

	void ShowDialogueUI() {
		dialogueBox.enabled = true;
		//advanceArrow.enabled = true;
		currentPortrait.enabled = true;
		dialogueText.enabled = true;
		speakerName.enabled = true;
	}

	void HideDialogueUI() {
		dialogueBox.enabled = false;
		advanceArrow.enabled = false;
		currentPortrait.enabled = false;
		dialogueText.enabled = false;
		speakerName.enabled = false;
		ClearText();
		ClearName();
	}

	void SetText(string str) {
		dialogueText.text = str;
	}

	void ClearText() {
		SetText("");
	}

	void SetPortrait(Sprite spr) {
		if (spr == null) {
			currentPortrait.enabled = false;
			return;
		}
		currentPortrait.sprite = spr;
	}

	void ClearPortrait() {
		currentPortrait.enabled = false;
	}

	void SetName(string name) {
		speakerName.text = name;
	}

	void ClearName() {
		SetName("");
	}

}
