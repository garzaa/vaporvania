using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {

	[HideInInspector] public GameObject playerObject;
	[HideInInspector] public Enemy mainController;
	[HideInInspector] public Rigidbody2D rb2d;

	//distance to the player at which to stop moving towards them
	public float seekThreshold = .2f;

	void Start() {
		mainController = this.gameObject.GetComponent<Enemy>();
		playerObject = GameObject.Find("Player");
		rb2d = this.GetComponent<Rigidbody2D>();
	} 

	public virtual void Move(){}
}
