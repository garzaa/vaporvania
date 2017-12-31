using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steven : NPC {

	public override void CreateDialogue() {
		List<string> testConvo = new List<string>();
		testConvo.Add("Nice sword!\nIt really ties the look together.");
		testConvo.Add("Really, though, I expected more.");
		testConvo.Add("!...");
		testConvo.Add("Suit yourself.");
		
		convos.Add(testConvo);

		testConvo = new List<string>();
		testConvo.Add("!So what's your deal?");
		testConvo.Add("I just like it down here. What's your deal?");
		testConvo.Add("!Trying to get back to the surface.");
		testConvo.Add("Good luck with that.");
		convos.Add(testConvo);

		testConvo = new List<string>();
		testConvo.Add("Shouldn't you be out killing my cousins or something?");
		convos.Add(testConvo);
	}
}
