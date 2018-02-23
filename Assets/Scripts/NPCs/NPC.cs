using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//edit 2d arrays in the inspector
[System.Serializable]
public class SerializableConversation {
	public SerializableLine[] lines;
}

[System.Serializable]
public class SerializableLine {
	public string text;
	public string name;
	public int imageIndex;
}

//basic NPC to talk to. maybe have a Shopkeeper to extend the NPC 
public class NPC : Interactable {

	//contains a list of sub-trees for actual dialogue lines
	public List<Conversation> convos;

	//keeping track of where they are in the conversation tree
	[HideInInspector]  public int currentConvo;
	[HideInInspector]  public int currentLine;

	[HideInInspector] public UIController uc;

	//make this a list, so they can change after certain lines
	public Sprite[] portraits;
	public string npcName;

	//since nothing is scripted these are just the basic dialogue lines
	public SerializableConversation[] editorConvos;


	void Start() {
		uc = GameObject.Find("GameController").GetComponent<UIController>();
		convos = new List<Conversation>();
		CreateDialogue();
		Initialize();

		CreateDialogueFromEditor();
	}

	//to be overwritten by individual NPC controllers.
	//using hardcoded, in-file lines this time because parsing db files is a massive pain
	public virtual void CreateDialogue() {

	}

	public override void Interact(GameObject player) {
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

	//default NPC name and portrait
	public DialogueLine MakeLine(string content) {
		return new DialogueLine(content, this.npcName, 0);
	}

	public DialogueLine MakeLine(SerializableLine line) {
		if (string.IsNullOrEmpty(line.name)) {
			line.name = this.npcName;
		}
		//if the image index is unspecified it defaults to zero, no need to catch it
		return new DialogueLine(line.text, line.name, line.imageIndex);
	}

	//called at the end of every conversation subtree when the dialogue is closed
	//can be a hook for NPC-specific functions
	public virtual void FinishLine(int convo, int line) {
		
	}

	public virtual void Initialize() {}

	public void CreateDialogueFromEditor() {
		if (editorConvos != null) {
			convos = new List<Conversation>();
			//for every conversation
			for (int i=0; i<editorConvos.Length; i++) {
				Conversation temp = new Conversation();
				//for every line in that conversation
				for (int j=0; j<editorConvos[i].lines.Length; j++) {
						temp.Add(MakeLine(editorConvos[i].lines[j]));
				}
				convos.Add(temp);
			}
		}
	}
}
