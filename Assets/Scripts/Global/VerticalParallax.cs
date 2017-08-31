using UnityEngine;
using System.Collections;

//does what it says on the tin
public class VerticalParallax : MonoBehaviour {

	//movement ratio to that of the player, 0 is stationary
    public float ratio;

    GameObject player;
    float playerOrigY;
    float origY;

    //new stuff
    float prevY;
    float currY;

    void Start () {
        player = GameObject.Find("Player");
        currY = player.transform.position.y;
        prevY = player.transform.position.y;
	}
	
	void Update () {
        if (ratio != 0)
        {

            currY = player.transform.position.y;

            this.transform.Translate(new Vector2(0, ratio * (currY - prevY)));

            prevY = player.transform.position.y;

        }
	}
}
