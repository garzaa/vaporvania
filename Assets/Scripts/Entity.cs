using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    private SpriteRenderer myRenderer;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;

    // Update is called once per frame
    void Update () {
		
	}

    public void Flash()
    {
        //grab the spriterenderer and toggle opacity with a coroutine
    }

    public void FlashWhite()
    {
        //toggle white sprite and normal sprite down below
    }

    //being lazy is valid :^)
    public void Log(string str)
    {
        Debug.Log(str);
    }

    void whiteSprite()
    {
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default"); // or whatever sprite shader is being used

        myRenderer.material.shader = shaderGUItext;
        myRenderer.color = Color.white;
    }

    void normalSprite()
    {
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default"); // or whatever sprite shader is being used

        myRenderer.material.shader = shaderSpritesDefault;
        myRenderer.color = Color.white;
    }
}
