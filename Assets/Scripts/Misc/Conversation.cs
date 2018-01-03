using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation {

	public List<DialogueLine> lines;

	public Conversation() {
		this.lines = new List<DialogueLine>();
	}

	public Conversation Add(DialogueLine dialogueLine) {
		lines.Add(dialogueLine);
		return this;
	}

	//overload bracket notation
	public DialogueLine this[int i] {
		get {
			return lines[i];
		}
		set {
			Debug.LogError("Don't set dialogue lines like that!!");
		}
	}

	public int Length() {
		return lines.Count;
	}

}
