using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steven : NPC {

	public override void CreateDialogue() {
		Conversation temp = new Conversation()
			.Add(MakeLine("Nice sword!\nIt really ties the look together."))
			.Add(MakeLine(". . .", -1))
			.Add(MakeLine("Suit yourself."));
		convos.Add(temp);

		temp = new Conversation()
			.Add(MakeLine("What's your deal, you get spit out by a Paradise lab?"))
			.Add(MakeLine("Looks like it, from the way you're dressed.\nMinus the big sword."))
			.Add(MakeLine("Just trying to get back to the surface.", -1))
			.Add(MakeLine("Good luck with that."));
		convos.Add(temp);

		temp = new Conversation()
			.Add(MakeLine("Shouldn't you be out killing my cousins or something?"));
		convos.Add(temp);
	}
}
