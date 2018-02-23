using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulNPC : NPC {

	GameCheckpoints gp;
	public string dependentState;
	public SerializableConversation[] stateConvos;

	public override void Initialize() {
		gp = GameObject.Find("GameController").GetComponent<GameCheckpoints>();
	}

	public override void Interact(GameObject player) {

		//override conversations on interact
		if (!string.IsNullOrEmpty(dependentState) && gp.CheckState(dependentState)) {
			print("beanis");
			this.editorConvos = this.stateConvos;
			CreateDialogue();
		}

		uc.OpenDialogue(this);

		//don't throw index errors at the end of a conversation tree
		if (currentConvo >= convos.Count) {
			currentConvo = convos.Count - 1;
		}

		uc.RenderDialogue(convos[currentConvo][currentLine]);
	}

	public override void CreateDialogue() {
		//then create the custom conversation list out of the dialogue lines
		if (editorConvos != null) {
			convos = new List<Conversation>();
			//for every conversation
			for (int i=0; i<editorConvos.Length; i++) {
				Conversation temp = new Conversation();
				//for every line in that conversation
				for (int j=0; j<editorConvos[i].lines.Length; j++) {
						//temp.Add(MakeLine(dialogueLines[i].lines[j]));
						temp.Add(MakeLine(editorConvos[i].lines[j]));
				}
				convos.Add(temp);
			}
		}
	}
}
