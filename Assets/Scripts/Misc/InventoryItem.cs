using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour {
	public string itemName;
	public string description;
	public Sprite sprite;
	public int count;

	//to be overridden by things like money
	//where even if you don't have any, it stays in your inventory
	public bool removeOnZero = true;
	
	public virtual void PickupEvent() {

	}
}
