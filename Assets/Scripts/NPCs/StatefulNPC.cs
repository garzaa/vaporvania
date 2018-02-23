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
			this.editorConvos = this.stateConvos;
			CreateDialogueFromEditor();
		}

		uc.OpenDialogue(this);

		//don't throw index errors at the end of a conversation tree
		if (currentConvo >= convos.Count) {
			currentConvo = convos.Count - 1;
		}

		uc.RenderDialogue(convos[currentConvo][currentLine]);
	}

}
