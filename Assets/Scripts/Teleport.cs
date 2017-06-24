using UnityEngine;

using System.Collections;

public class Teleport : MonoBehaviour {

	public Transform destination;

	void OnCollisionStay2D(Collision2D col) 
	{
		if(col.collider.gameObject.tag == "Player" && Input.GetKeyDown (KeyCode.DownArrow))
		{
			col.collider.gameObject.transform.position = destination.position;
		}
	}
}