using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour {
	public string itemName;
	public string description;
	public Sprite sprite;
	
	public virtual void PickupEvent() {

	}
}
