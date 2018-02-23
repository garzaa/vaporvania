using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCheckpoints : MonoBehaviour {
	List<string> internalStates;

	void Start() {
		internalStates = new List<string>();
	}

	public bool CheckState(string state) {
		return internalStates.Contains(state);
	}

	public bool AddState(string state) {
		if (CheckState(state)) {
			return false;
		} else {
			internalStates.Add(state);
			return true;
		}
	}
}

// ok to embed this here, it's only used once across the game
public abstract class Checkpoints {
	public const string FoughtFirstSludge = "FoughtFirstSludge";
}
