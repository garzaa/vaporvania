using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//basic NPC to talk to. maybe have a Shopkeeper to extend the NPC 
public class NPC : Interactable {

	//dialogue lines for when the player walks past (use a trigger)
	public List<string> ambientLines;

	//then a list of sub-trees for actual dialogue lines
	public List<Conversation> convos;

	//keeping track of where they are in the conversation tree
	public int currentConvo;
	public int currentLine;

	PlayerController pc;

	//make this a list, so they can change after certain lines
	public Sprite[] portraits;
	public string npcName;

	UIController uc;
	Inventory inventory;

	public InventoryItem[] npcItems;

	void Start() {
		uc = GameObject.Find("GameController").GetComponent<UIController>();
		inventory = GameObject.Find("GameController").GetComponent<Inventory>();
		ambientLines = new List<string>();
		convos = new List<Conversation>();
		CreateDialogue();
	}

	void GetPlayer() {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
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

		uc.RenderDialogue(convos[currentConvo][currentLine]);
	}

	//to be called by UIController on player pressing enter if a dialogue box is open
	public void AdvanceLine() {
		//if at the last line, the UI controller will close everything and unlink from this NPC
		if (++currentLine == convos[currentConvo].Length()) {
			uc.CloseDialogue();
			currentConvo++;
			currentLine = 0;
			//hook for the NPC that extends this script
			FinishLine(currentConvo, currentLine);
			return;
		}

		FinishLine(currentConvo, currentLine);

		uc.RenderDialogue(convos[currentConvo][currentLine]);
	}

	public bool HasNext() {
		return (currentLine+1 < convos[currentConvo].Length());
	}

	//called at the end of every conversation subtree when the dialogue is closed
	//can be a hook for NPC-specific functions
	public virtual void FinishLine(int convo, int line) {
		
	}

	//default NPC name and portrait
	public DialogueLine MakeLine(string content) {
		return new DialogueLine(content, this.npcName, 0);
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
