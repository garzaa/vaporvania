using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//basic NPC to talk to. maybe have a Shopkeeper to extend the NPC 
public class NPC : Interactable {

	//dialogue lines for when the player walks past (use a trigger)
	List<string> ambientLines;

	//then a list of sub-trees for actual dialogue lines
	List<List<string>> convos;

	//keeping track of where they are in the conversation tree
	int currentConvo;
	int currentLine;

	PlayerController pc;

	//make this a list, so they can change after certain lines
	public Image[] portraits;
	public string npcName;

	void Start() {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}

	//to be overwritten by individual NPC controllers.
	//using hardcoded, in-file lines this time because parsing db files is a massive pain
	public virtual void AddLines() {

	}

	void OpenDialogue() {
		pc.Freeze();
		//then do whatever
		//call the UIController here though, don't directly interact with the UI system
	}

	void CloseDialogue() {
		//finish up UI things
		//in the eventual UIController, unlink this NPC from the current thing being talked to

		pc.UnFreeze();
	}

	public override void Interact(GameObject player) {
		//TODO
		//open dialogue ui stuff
		//write a line
		//then listen for input?
		//or have some UI controller that listens for inputs and then forwards the signal to the NPC
		//so then on a LineForward() then advance the current line
	}

	//to be called by UIController on player pressing enter if a dialogue box is open
	public void AdvanceLine() {
		FinishLine(currentConvo, currentLine);
		//if at the last line
		if (++currentLine == convos[currentConvo].Count) {
			CloseDialogue();
		}
		//then the UI controller will write a new line if there's still one available
	}

	//called at the end of every conversation subtree when the dialogue is closed
	//can be a hook for NPC-specific functions
	public virtual void FinishLine(int convo, int line) {
		
	}
}
