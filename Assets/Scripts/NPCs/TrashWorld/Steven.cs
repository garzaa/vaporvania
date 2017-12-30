using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steven : NPC {

	public override void CreateDialogue() {
		List<string> testConvo = new List<string>();
		testConvo.Add("Nice sword!\nIt really ties the look together.");
		testConvo.Add("Really, though, I expected more from one of you.");
		testConvo.Add("!...");
		
		convos.Add(testConvo);

		testConvo = new List<string>();
		testConvo.Add("!So what's your deal?");
		testConvo.Add("I just like it down here. What's your deal?");
		testConvo.Add("!I don't know.");
		testConvo.Add("Well alright then.");
		convos.Add(testConvo);

		testConvo = new List<string>();
		testConvo.Add("Why are you still here? Let me smoke in peace.");
		convos.Add(testConvo);
	}
}
