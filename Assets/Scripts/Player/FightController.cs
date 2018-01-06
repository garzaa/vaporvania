using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightController : MonoBehaviour {

	PlayerController pc;
	Animator anim;

	void Start() {
		pc = GetComponent<PlayerController>();
		anim = GetComponent<Animator>();
	}

	public void Attack() {
        if (pc.attackCooldown || pc.parrying || pc.frozen) {
            return;
        }

        if (Input.GetKey(KeyCode.X) && !Input.GetKey(KeyCode.DownArrow) && !pc.swinging && !pc.dashing && pc.grounded) {
            Parry();
        } 
        
        else if (Input.GetKeyDown(KeyCode.D) && !pc.invincible) {
            pc.Die();
        }

        else if (Input.GetKeyDown(KeyCode.Z) && pc.CanGroundAttack()) {
            //in case of canceling another attack somehow
            //InterruptAttack();   
            anim.SetTrigger("groundAttack");
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
}
