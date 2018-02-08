using System.Collections;
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
	Boss currentBoss;
	//these are to be hooked up in the editor along with heart containers as above
	//TODO: assign this and make dialogueOpen a function that checks whether it's active
	public GameObject dialogueContainer;
	public Image dialogueBox;
	public Text dialogueText;
	public Image currentPortrait;
	public Image advanceArrow;
	public Sprite playerPortrait;
	public Text speakerName;

	//signs
	Sign currentSign;
	public Sprite signPortrait;

	bool openedDialogueThisFrame = false;

	Inventory inventory;
	public Transform inventoryUI;

	public Text alertText;

	void Start() {
		gc = GetComponent<GameController>();
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
		inventory = GetComponent<Inventory>();
		HideDialogueUI();
		ClearText();
		ClearPortrait();
		CloseInventory();
	}

	void Update() {
		if (DialogueOpen() && !pc.frozen) {
			FreezePlayer();
		}
		UpdateUI();
		CheckForLineAdvance();
		//don't immediately advance dialogue on the first opening
		if (openedDialogueThisFrame) {
			openedDialogueThisFrame = false;
		}

		CheckInventoryOpen();
	}

	void CheckInventoryOpen() {
		if (Input.GetKeyDown(KeyCode.Tab) && !DialogueOpen()) {
			if (!InventoryOpen()) {
				OpenInventory();
			} else {
				CloseInventory();
			}
		}
	}

	void OpenInventory() {
		FreezePlayer();
		PopulateInventory();
		inventoryUI.gameObject.SetActive(true);
	}

	void CloseInventory() {
		inventoryUI.gameObject.SetActive(false);
		UnFreezePlayer();
	}

	void UpdateUI() {
		UpdateHealth();
	}

	void CheckForLineAdvance() {
		if (openedDialogueThisFrame) {
			openedDialogueThisFrame = false;
			return;
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			if (currentBoss != null) {
				currentBoss.AdvanceLine();
			}
			else if (currentNPC != null) {
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
		if (DialogueOpen()) return;
		openedDialogueThisFrame = true;
		this.currentNPC = npc;
		FreezePlayer();
		SetPortrait(npc.portraits[0]);
		ShowDialogueUI();
	}

	public void OpenDialogue(Sign sign) {
		if (DialogueOpen()) return;
		openedDialogueThisFrame = true;
		currentSign = sign;
		if (sign.portrait != null) {
			SetPortrait(sign.portrait);
		} else {
			SetPortrait(signPortrait);
		}

		if (!string.IsNullOrEmpty(sign.signName)) {
			SetName(sign.signName);
		}

		FreezePlayer();
		ShowDialogueUI();
	}

	public void OpenDialogue(Boss boss) {
		if (DialogueOpen()) return;
		openedDialogueThisFrame = true;
		currentBoss = boss;
		FreezePlayer();
		SetName(boss.bossName);
		SetPortrait(boss.bossPortraits[0]);
		ShowDialogueUI();
	}

	public void FreezePlayer() {
		pc.Freeze();
		pc.ZeroVelocity();
		pc.InterruptAttack();
		pc.InterruptDash();
		pc.SetInvincible(true);
	}

	public void UnFreezePlayer() {
		pc.UnFreeze();
		pc.SetInvincible(false);
	}

	//called by the NPC controller if the NPC is out of dialogue
	public void CloseDialogue() {
		UnFreezePlayer();
		this.currentNPC = null;
		this.currentSign = null;
		HideDialogueUI();
		if (currentBoss != null) {
			currentBoss.StopTalking();
			currentBoss = null;
		}
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
			if (currentNPC != null) {
				SetPortrait(currentNPC.portraits[line.image]);
			} else if (currentBoss != null) {
				SetPortrait(currentBoss.bossPortraits[line.image]);
			}
			SetName(line.name);
		}
		SetText(line.text);
	}

	void ShowDialogueUI() {
		dialogueContainer.SetActive(true);
		dialogueBox.enabled = true;
		//advanceArrow.enabled = true;
		currentPortrait.enabled = true;
		dialogueText.enabled = true;
		speakerName.enabled = true;
	}

	void HideDialogueUI() {
		dialogueContainer.SetActive(false);
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

	bool InventoryOpen() {
		return inventoryUI.gameObject.activeSelf;
	}

	void PopulateInventory() {
		/*
		inventory is structured like this:
		inventoryUI
			itemsList
				item
					itemSprite
					itemName
					itemText
		 */
		
		//update the inventory with the current items
		int itemCount = 0;
		List<InventoryItem> allItems = inventory.GetAll();

		int itemsPerPage = inventoryUI.childCount;

		foreach (Transform itemParent in inventoryUI.Find("itemsList")) {
			//populate it with the appropriate item from the inventory
			InventoryItem currItem = allItems[itemCount];
			PopulateItemInfo(itemParent, currItem);
			itemCount++;
			
			if (itemCount > itemsPerPage) {
				break;
			}
		}
	}

	void PopulateItemInfo(Transform itemTree, InventoryItem item) {
		itemTree.Find("itemSprite").GetComponent<Image>().sprite = item.sprite;
		itemTree.Find("itemName").GetComponent<Text>().text = item.itemName;
		itemTree.Find("itemText").GetComponent<Text>().text = item.description;
	}

	bool DialogueOpen() {
		return dialogueContainer.activeSelf;
	}

	public void DisplayAlert(Alert incomingAlert) {
		if (AlertActive() || incomingAlert.priority) {
			alertText.text = incomingAlert.content;
			alertText.GetComponent<Animator>().SetTrigger("flashAlert");
		}
	}

	bool AlertActive() {
		//return the active status of the entire gameobject
		//because the text itself will be toggled on and off via an attached animation
		return alertText.gameObject.activeSelf;
	}
}
