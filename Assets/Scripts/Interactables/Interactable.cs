using UnityEngine;

public class Interactable : MonoBehaviour {

	public GameObject promptPrefab;
	GameObject currentPrompt = null;

	//how far above a sprite to drop the prompt object
	//since we're working with 108 ppu, this is three pixels
	const float TOP_MARGIN = 3f/108f;

    //adds and removes a little animated prompt prefab above the object sprite
    //check if one exists, etc
    public GameObject AddPrompt() {
		if (currentPrompt == null) {
			//if there's a sprite renderer on this object, stick a prompt a little ways on top of it
			if (gameObject.GetComponent<SpriteRenderer>() != null) {
				SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
				float upperBound = spr.bounds.max.y;
				float yPos = upperBound + TOP_MARGIN + promptPrefab.GetComponent<SpriteRenderer>().bounds.extents.y;
				currentPrompt = (GameObject) Instantiate(promptPrefab, new Vector2(this.transform.position.x, yPos), Quaternion.identity);
			} 
			//otherwise just do it above the gameobject's center
			else {
				currentPrompt = (GameObject) Instantiate(promptPrefab, new Vector2(this.transform.position.x, this.transform.position.y + TOP_MARGIN), Quaternion.identity);
			}
		}

		currentPrompt.transform.SetParent(this.transform);
		return currentPrompt;
	}

	public void RemovePrompt() {
		if (currentPrompt) {
			Destroy(currentPrompt.gameObject);
		}
	}

	public virtual void Interact(GameObject player) {
		//to be extended by child classes
	}

	void Start() {
		this.gameObject.layer = LayerMask.NameToLayer("Interactables");
	}
}
