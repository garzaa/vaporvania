using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxController : MonoBehaviour {
	public int damage = 1;
	public float hitstop = .04f;
	public bool cameraShake = false;
	public Vector2 knockbackVector = new Vector2(3, 1);
}
