using UnityEngine;
using System.Collections;

/// <summary>
/// usage: attach this to one object, have another one
/// named the same thing but with "2" at the end, only
/// the position of the first one matters
/// 
/// this script takes two gameobjects and places them
/// side by side. when one moves off the screen it's
/// put in front of the other so they repeat
/// 
/// backgroundAnchor is what to use for the screen borders
/// 
/// </summary>

public class HorizontalRepeater : MonoBehaviour {

	//optional move speed, leave at 0 for parallax-enabled objectss
    public float speed = 0;

    //this overlaps the two sprites so there isn't any flickering 
    //if they move slowly, .002 is usually fine
    public float overlap = .00f;

	float horizMin;
	float horizMax;
    public GameObject other;
    SpriteRenderer sr;

	GameObject mainCamera;
	float cameraSize;

	// Use this for initialization
    void Start () {

        //if (other == null) other = GameObject.Find(name + "2");
      
        //get the left edge of the camera
		mainCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
		cameraSize = mainCamera.GetComponent<Camera>().orthographicSize *2;

        sr = other.GetComponent<SpriteRenderer>();
        //other.transform.position = new Vector3(2 * (temp.bounds.max.x - temp.bounds.min.x), this.transform.position.y, this.transform.position.z);

	}
	
	// Update is called once per frame
	void Update () {
		//TODO: offload this to a CameraController at some point so this isn't called for every repeating bg (if that actually becomes an issue)
		horizMin = mainCamera.transform.position.x - cameraSize;
		horizMax = mainCamera.transform.position.x + cameraSize;

	    if (speed != 0)
        {
            gameObject.transform.Translate(-speed, 0, 0);
            other.transform.Translate(-speed, 0, 0);

        }

		//if one is off the screen move it up
		if (gameObject.GetComponent<SpriteRenderer>().bounds.max.x < horizMin && gameObject.transform.position.x < mainCamera.transform.position.x)
		{
			sr = gameObject.GetComponent<SpriteRenderer>();
			float length = sr.bounds.max.x - sr.bounds.min.x;
			gameObject.transform.Translate(2 * length - overlap, 0, 0);
		} 
		else if (gameObject.GetComponent<SpriteRenderer>().bounds.min.x > horizMax && gameObject.transform.position.x > mainCamera.transform.position.x)
		{
			sr = gameObject.GetComponent<SpriteRenderer>();
			float length = sr.bounds.max.x - sr.bounds.min.x;
			gameObject.transform.Translate(-2 * length + overlap, 0, 0);
		}

		if (other.GetComponent<SpriteRenderer>().bounds.max.x < horizMin && other.transform.position.x < mainCamera.transform.position.x)
		{
			sr = other.GetComponent<SpriteRenderer>();
			float length = sr.bounds.max.x - sr.bounds.min.x;
			other.transform.Translate(2 * length - overlap, 0, 0);
		}
		else if (other.GetComponent<SpriteRenderer>().bounds.min.x > horizMax && other.transform.position.x > mainCamera.transform.position.x)
		{
			sr = other.GetComponent<SpriteRenderer>();
			float length = sr.bounds.max.x - sr.bounds.min.x;
			other.transform.Translate(-2 * length + overlap, 0, 0);
		}
		
    }
}
