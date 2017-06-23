using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Flash()
    {
        //grab the spriterenderer and toggle opacity with a coroutine
    }

    //being lazy is valid :^)
    public void Log(string str)
    {
        Debug.Log(str);
    }
}
