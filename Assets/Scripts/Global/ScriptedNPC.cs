using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedNPC : NPC {

	Inventory inventory;

	public InventoryItem[] npcItems;

	public override void Initialize() {
		inventory = GameObject.Find("GameController").GetComponent<Inventory>();
	}

	//default NPC name
	//if the image is <0, then it'll just use the player name
	public DialogueLine MakeLine(string content, int image) {
		return new DialogueLine(content, "", image);
	}

	//making all the info
	public DialogueLine MakeLine(string content, string name, int image) {
		return new DialogueLine(content, name, image);
	}
}
