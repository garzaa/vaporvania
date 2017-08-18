using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour {

	GameObject parentObject;

	public GameObject hitmarker;

	void Start() {
		parentObject = this.gameObject.transform.parent.gameObject;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag.Equals("playerAttack")) {
			parentObject.GetComponent<Enemy>().OnHit(other.gameObject.GetComponent<BoxCollider2D>());
			StartCoroutine(Hitstop());
			//instantiate a hitmarker at the point of contact
			//this works for tiny enemies, we might have to have multiple hitboxes on bosses (or SOMETHING else with dynamically calculating 
			//the collision midway point based on relative positions of the two hitboxes)
			Instantiate(Resources.Load("Prefabs/Hitmarker"), new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
		}
	}

	//pause animations for both entities
	IEnumerator Hitstop() {

		bool frozePlayer = false;

		//store the last velocities
		Rigidbody2D rb2d = parentObject.GetComponent<Rigidbody2D>();
		GameObject playerObject = parentObject.GetComponent<Enemy>().playerObject;
		Vector2 lastV = rb2d.velocity;
		Vector2 lastPlayerV = playerObject.GetComponent<Rigidbody2D>().velocity;

		//freeze the positions, don't want to do it if the player is hitting multiple entities
		parentObject.GetComponent<Enemy>().FreezeInSpace();
		if (!playerObject.GetComponent<PlayerController>().inHitstop) {
			playerObject.GetComponent<PlayerController>().FreezeInSpace();
			frozePlayer = true;
		}

		//freeze the animations
		if (parentObject.GetComponent<Animator>() != null) {
			parentObject.GetComponent<Animator>().speed = 0;
		}
		playerObject.GetComponent<Animator>().speed = 0;

		yield return new WaitForSeconds(.05f);

		//then undo everything
		if (parentObject.GetComponent<Animator>() != null) {
			parentObject.GetComponent<Animator>().speed = 1;
		}
		playerObject.GetComponent<Animator>().speed = 1;

		parentObject.GetComponent<Enemy>().UnFreezeInSpace();
		if (frozePlayer) {
			playerObject.GetComponent<PlayerController>().UnFreezeInSpace();
			frozePlayer = true;
		}

		playerObject.GetComponent<Rigidbody2D>().velocity = lastPlayerV;
		rb2d.velocity = lastV;
	}


}
