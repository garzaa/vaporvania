using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour 
{
	private BoxCollider2D bc;
	private PlayerController pc;

	void Start () 
	{
		/*
		bc = GetComponent<BoxCollider2D> ();
		pc = GameObject.Find ("Player").GetComponent<PlayerController> ();
		bc.isTrigger = true;
		*/
	}

	void Update () 
	{
		
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.collider.gameObject.tag != "player")
		{
			//print ("hit!");
		}
	}
}
