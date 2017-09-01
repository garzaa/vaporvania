using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    SpriteRenderer myRenderer;
    Shader shaderGUItext;
    Shader shaderSpritesDefault;

    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool movingRight = false;
    public bool frozen = false;

    GameObject dustSprite;

    //being lazy is valid :^)
    public void Log(string str)
    {
        Debug.Log(str);
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

    public void CheckFlip() {
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null) {
            if (!facingRight && rb2d.velocity.x > 0 && movingRight)
            {
                Flip();
            }
            else if (facingRight && rb2d.velocity.x < 0 && !movingRight)
            {
                Flip();
            }
        }
    }

    public void FreezeInSpace() {
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null) {
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void UnFreezeInSpace() {
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null) {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void createDust() {
        dustSprite = (GameObject) Resources.Load("Prefabs/TempEffects/Dust");
        SpriteRenderer spr = this.GetComponent<SpriteRenderer>();
        Instantiate(dustSprite, new Vector2(spr.transform.position.x, spr.bounds.min.y), Quaternion.identity);
    }
}
