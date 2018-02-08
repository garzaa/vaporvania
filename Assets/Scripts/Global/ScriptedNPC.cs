using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedNPC : NPC {

	Inventory inventory;

	public InventoryItem[] npcItems;

	public override void Initialize() {
		inventory = GameObject.Find("GameController").GetComponent<Inventory>();
	}
}
