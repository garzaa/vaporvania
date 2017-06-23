using UnityEngine;

using System.Collections;

public class Teleport : MonoBehaviour {

	public Transform destination;

	void OnCollisionEnter(Collision col) 
	{
		if(col.collider.gameObject.tag == "Player")
		{
			col.collider.gameObject.transform.position = destination.position;
		}
	}

}