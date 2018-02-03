using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour {

	PlayerController pc;
	Animator anim;

    //meteor blade things
    public bool inMeteor;
    public GameObject jets;
    public Transform impactPrefab;

	void Start() {
		pc = GetComponent<PlayerController>();
		anim = GetComponent<Animator>();
	}

	public void Attack() {
        if (pc.attackCooldown || pc.parrying || pc.frozen) {
            return;
        }

        //meteor blade
        if (!pc.grounded && !pc.dashing && Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.DownArrow)) {
            MeteorBlade();
        }

        if (Input.GetKey(KeyCode.X) && !Input.GetKey(KeyCode.DownArrow) && !pc.swinging && !pc.dashing && pc.grounded) {
            Parry();
        } 

        else if (Input.GetKeyDown(KeyCode.Z) && pc.CanGroundAttack()) {
            if (Input.GetKey(KeyCode.UpArrow)) {
                anim.SetTrigger("upTilt");
            } else {
                anim.SetTrigger("groundAttack");
            }
		} 
        
        //neutral-air vs down-air
        else if (Input.GetKeyDown(KeyCode.Z) && 
            !(Input.GetKey(KeyCode.DownArrow)) &&
            CanAirAttack()) {
            AirAttack();
        } else if (Input.GetKeyDown(KeyCode.Z) && 
            (Input.GetKey(KeyCode.DownArrow)) &&
            CanAirAttack()) {
            DownAir();
        }

        //can be generous with key checking here for reasons below
        else if ((Input.GetKey(KeyCode.X) && Input.GetKeyDown(KeyCode.DownArrow))
                || (Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.DownArrow))) {
            pc.Dodge();
        }

        //on pressing shift, wait a few frames to dash to give the player a window to press Z to parry
        //the dash timwout won't be started if the player has already started parrying, which is checked for at the start of this function
        else if (pc.HorizontalInput() && Input.GetKeyDown(KeyCode.LeftShift)) {
            pc.Dash();
        }
        
	}

    void Parry() {
        pc.InterruptAttack();
        anim.SetTrigger("parry");
    }

	void AirAttack() {
        anim.SetTrigger("airAttack");
    }

    void DownAir() {
        anim.SetTrigger("downAir");
    }

    bool CanAirAttack() {
        return (!pc.grounded && !pc.swinging && !pc.wallSliding && !pc.dashing);
    }

    void MeteorBlade() {
        pc.Freeze();
        pc.SetInvincible(true);
        pc.rb2d.velocity = new Vector2(0, pc.TERMINAL_VELOCITY);
        anim.SetTrigger("meteorBlade");
        this.inMeteor = true;
        //turn on jets
        jets.SetActive(true);
        foreach (Animator childAnim in jets.GetComponentsInChildren<Animator>()) {
            childAnim.Play("Activate", -1, 0f);
        }
    }

    //called when the player hits the ground
    //animation transitions are taken care of automatically with the grounded bool
    public void LandMeteorBlade(Collision2D col) {

        //edge case handling if the player gets bounced up from envdamage
        if (!inMeteor) {
            return;
        }
        
        inMeteor = false;

        Instantiate(impactPrefab, pc.GetBottomCenter(), Quaternion.identity);
        pc.cameraShaker.SmallShake();
    }

    //called when the impact animation finishes before the transition to sword sheathing
    public void StopMeteorBlade() {
        this.inMeteor = false;
        pc.UnFreeze();
        pc.SetInvincible(false);
    }
}
