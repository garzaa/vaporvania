using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NPCs pass these individually to UIController when opening dialogue
public class DialogueLine {
	public string text;

	//-1 for the player, otherwise an index in the image[] that the parent NPC class contains
	public int image;

	//for variable names during dialogue
	public string name;

	public DialogueLine(string text, string name, int image=0) {
		this.text = text;
		this.image = image;
		this.name = name;
	}
}
