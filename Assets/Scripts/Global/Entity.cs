using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    SpriteRenderer myRenderer;
    Shader shaderGUItext;
    Shader shaderSpritesDefault;

    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool movingRight = false;
    [HideInInspector] public bool frozen = false;

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

    
    public void Flip() 
	{
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        //flip by scaling -1
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }
}
