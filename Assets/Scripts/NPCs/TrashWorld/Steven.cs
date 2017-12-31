using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steven : NPC {

	public override void CreateDialogue() {
		List<string> testConvo = new List<string>();
		testConvo.Add("Nice sword!\nIt really ties the look together.");
		testConvo.Add("!. . .");
		testConvo.Add("Suit yourself.");
		
		convos.Add(testConvo);

		testConvo = new List<string>();
		testConvo.Add("What's your deal, you get spit out by a Paradise lab?");
		testConvo.Add("Looks like it, from the way you're dressed.\nMinus the big sword.");
		testConvo.Add("!Just trying to get back to the surface.");
		testConvo.Add("Good luck with that.");
		convos.Add(testConvo);

		testConvo = new List<string>();
		testConvo.Add("Shouldn't you be out killing my cousins or something?");
		convos.Add(testConvo);
	}
}
