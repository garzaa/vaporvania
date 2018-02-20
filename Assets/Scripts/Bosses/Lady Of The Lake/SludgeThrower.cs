using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SludgeThrower : MonoBehaviour {

	//for the sludge throwing attack
    public GameObject thrownSludge;
    //initial vector for throwing sludges, the y value will decrease and she'll throw three at the player
    public Vector2 sludgeVector;
    public Transform sludgePoint;

	public void ThrowSludgeball(int sludgeNum) {
        GameObject sludge = (GameObject) Instantiate(thrownSludge, sludgePoint.position, Quaternion.identity);
        float xVec = sludgeVector.x;
        float yVec = sludgeVector.y;
        sludge.GetComponent<Rigidbody2D>().velocity = new Vector2(-xVec, yVec / sludgeNum);
    }
}
