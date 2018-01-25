using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
	List<InventoryItem> items;

	public InventoryItem[] initialItems;

	void Start() {
		items = new List<InventoryItem>();
		//add the sword and other starting items
		foreach (InventoryItem i in initialItems) {
			items.Add(i);
		}
	}

	public bool Contains(InventoryItem item) {
		foreach (InventoryItem i in items) {
			if (i.itemName.Equals(item.name)) {
				return true;
			}
		}
		return false;
	}

	public bool ContainsName(string name) {
		return GetByName(name) != null;
	}

	public InventoryItem GetByName(string name) {
		foreach (InventoryItem i in items) {
			if (i.itemName.Equals(name)) {
				return i;
			}
		}
		return null;
	}

	public void Add(InventoryItem item) {
		if (!Contains(item)) {
			items.Add(item);
		} else {
			Debug.LogWarning("Item "+item.itemName+" already in inventory!");
		}
	}

	public void Remove(string name) {
		foreach (InventoryItem i in items) {
			if (i.itemName.Equals(name)) {
				items.Remove(i);
			}
		}
	}

	public List<InventoryItem> GetAll() {
		return items;
	}

	//public List<InventoryItem> ListAbilities() ?	
}
