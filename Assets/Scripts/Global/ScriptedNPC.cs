using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedNPC : NPC {

	Inventory inventory;
	[HideInInspector] PlayerController pc;

	public InventoryItem[] npcItems;

	public override void Initialize() {
		inventory = GameObject.Find("GameController").GetComponent<Inventory>();
	}

	public void GetPlayer() {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}
}
