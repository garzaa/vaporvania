using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//basic NPC to talk to. maybe have a Shopkeeper to extend the NPC 
public class NPC : Interactable {

	//dialogue lines for when the player walks past (use a trigger)
	public List<string> ambientLines;

	//then a list of sub-trees for actual dialogue lines
	public List<List<string>> convos;

	//keeping track of where they are in the conversation tree
	public int currentConvo;
	public int currentLine;

	PlayerController pc;

	//make this a list, so they can change after certain lines
	public Sprite[] portraits;
	public string npcName;

	UIController uc;

	void Start() {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
		uc = GameObject.Find("GameController").GetComponent<UIController>();
		ambientLines = new List<string>();
		convos = new List<List<string>>();
		CreateDialogue();
	}

	//to be overwritten by individual NPC controllers.
	//using hardcoded, in-file lines this time because parsing db files is a massive pain
	public virtual void CreateDialogue() {

	}

	public override void Interact(GameObject player) {
		//TODO
		//open dialogue ui stuff
		//write a line
		//then listen for input?
		//or have some UI controller that listens for inputs and then forwards the signal to the NPC
		//so then on a LineForward() then advance the current line
		uc.OpenDialogue(this);

		//don't throw index errors at the end of a conversation tree
		if (currentConvo >= convos.Count) {
			currentConvo = convos.Count - 1;
		}

		uc.RenderLine(convos[currentConvo][currentLine]);
	}

	//to be called by UIController on player pressing enter if a dialogue box is open
	public void AdvanceLine() {
		//if at the last line, the UI controller will close everything and unlink from this NPC
		print("current line: " + (currentLine) + "\n conversation length: " + convos[currentConvo].Count);
		if (++currentLine == convos[currentConvo].Count) {
			uc.CloseDialogue();
			currentConvo++;
			currentLine = 0;
			//hook for the NPC that extends this script
			FinishLine(currentConvo, currentLine);
			return;
		}

		FinishLine(currentConvo, currentLine);

		print(currentConvo);
		print(currentLine);
		uc.RenderLine(convos[currentConvo][currentLine]);
	}

	public bool HasNext() {
		return (currentLine+1 < convos[currentConvo].Count);
	}

	//called at the end of every conversation subtree when the dialogue is closed
	//can be a hook for NPC-specific functions
	public virtual void FinishLine(int convo, int line) {
		
	}
}
