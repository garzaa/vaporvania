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
    public bool frozenInSpace = false;

    public GameObject dustSprite;

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
            this.frozenInSpace = true;
        }
    }

    public void UnFreezeInSpace() {
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null) {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            this.frozenInSpace = false;
        }
    }

    public void CreateDust() {
        //dustSprite = (GameObject) Resources.Load("Prefabs/Particles/Dust");
        Collider2D bc = this.GetComponent<Collider2D>();
        Instantiate(dustSprite, new Vector2(bc.transform.position.x, bc.bounds.min.y), Quaternion.identity);
    }
}
