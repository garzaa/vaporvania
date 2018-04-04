using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// inherited by other inventory components like health, dialogue, letterboxes, etc
public class UIComponent : MonoBehaviour {

	public void Show() {
		this.gameObject.SetActive(true);
	}

	public void Hide() {
		this.gameObject.SetActive(false);
	}
	
}
