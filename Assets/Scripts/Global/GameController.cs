using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public PlayerController pc;
	int lastHealth;
	Vector2 playerRespawnPoint;
	string playerRespawnScene = null;

	GameObject savePoint;
	bool saving = false;

	TransitionController tc;

	bool toRespawn = false;
	public string teleportTarget = null;
	
	//game checkpoints for NPCs and such
	List<string> gameCheckpoints;
	UIController uc;
	
	void Start() {
		tc = GetComponent<TransitionController>();
		if (SceneManager.GetActiveScene().name == "start") {
			tc.LoadSceneFade("Tutorial");
			teleportTarget = "startPoint";
		}

		gameCheckpoints = new List<string>();
		uc = GetComponent<UIController>();
	}

	// Update is called once per frame
	void Update() {

		//show the player once the save point animation is done
		if (this.savePoint != null) {

			if (savePoint.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("saving")) {
				saving = true;
			}

			//if saving has started, then an idle state means it's completed
			else if (saving && savePoint.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				pc.Show();
				pc.UnFreeze();
				pc.SetInvincible(false);
				//then unlink the save point and stop checking for saving
				this.savePoint = null;
				this.saving = false;
			}
		}
	}

	public void Save(GameObject sp) {
		//also need to find a way to freeze the player until the animation is finished
		//also hide them, but there's already a method for that
		if (pc.frozen) {
			return;
		}
		//then lock further inputs
		pc.Freeze();
		this.savePoint = sp;
		pc.Hide();
		//then move the player character to the save point so it doesn't look weird
		pc.transform.position = new Vector3(sp.transform.position.x, pc.transform.position.y, pc.transform.position.z);
		//also, cancel their momentum
		pc.rb2d.velocity = Vector3.zero;
		pc.SetInvincible(true);
		//finally, heal
		pc.FullHeal();
		if (savePoint.GetComponent<Animator>() != null) {
			savePoint.GetComponent<Animator>().SetTrigger("save");
		}
		//only set X to prevent weird grounding issues
		playerRespawnPoint = new Vector2(savePoint.transform.position.x, pc.transform.position.y);
		playerRespawnScene = SceneManager.GetActiveScene().name;

		//then display an alert
		uc.DisplayAlert(new Alert("GAME SAVED"));
	}

	//lets an interactable know to put an arrow above it
	public void AddPrompt(Interactable i) {
		i.AddPrompt();
	}

	public void RemovePrompt(Interactable i) {
		i.RemovePrompt();
	}

	public void Respawn() {
		pc.Freeze();
		if (SceneManager.GetActiveScene().name.Equals(playerRespawnScene)) {
			pc.transform.position = playerRespawnPoint;
			pc.Respawn();
		} else {
			//this means save point is in another scene, when scene transitions are a thing then load it based on the path
			//and then move the player to the last respawn point
			tc.LoadSceneFade(playerRespawnScene);
			toRespawn = true;
		}
	}

	void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

	public void OverrideSavePoint(Transform t) {
		playerRespawnPoint = t.position;
		playerRespawnScene = SceneManager.GetActiveScene().name;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		pc.ClearInteractables();

		//hacky workaround so the player doesn't return to the start scene on death
		if (SceneManager.GetActiveScene().name == "Tutorial") {
			playerRespawnPoint = pc.transform.position;
			playerRespawnScene = SceneManager.GetActiveScene().name;
		}
		
		//on level loaded, move the player back to their original spawn point	
		if (toRespawn) {
			print("right scene loaded, triggering respawn");
			Respawn();
			toRespawn = false;
		}
		else if (!string.IsNullOrEmpty(teleportTarget)) {
			//if we're moving to a new TP location instead
			pc.transform.position = GameObject.Find(teleportTarget).transform.position;
			teleportTarget = null;
			pc.UnFreeze();
			pc.UnFreezeInSpace();
		}
	}

	public bool HasCheckpoint(string check) {
		return gameCheckpoints.Contains(check);
	}
}
