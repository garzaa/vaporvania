﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyOfTheLake : Boss {

    public int initialEyeCount;
    public int eyeCount;
    public GameObject eyeContainer;

    //this needs to be a broken prefab since it'll have an animation controller attached to it
    //or just nest it in an empty gameobject, actually, and then change its animator
    public GameObject tiledSludgeContainer;

    Animator containerAnimator;

    public override void Initialize() {
        monologue = new List<DialogueLine>();
        AddLines();
        initialEyeCount = eyeContainer.transform.childCount;
        eyeCount = eyeContainer.transform.childCount;
        uc = GameObject.Find("GameController").GetComponent<UIController>();
        gc = uc.GetComponent<GameController>();
        containerAnimator = transform.parent.GetComponent<Animator>();
    }


    public override void BossMove() {
        if (!fighting || !moving) return;
        // 1/3 of a chance to flood the stage, since it's annoying
        if (Random.Range(0, 3) < 2) {
            ThrowSludges();
        } else {
            FloodStage();
        }
    }

    //move back and throw three sludges at the player
    public void ThrowSludges() {
        moving = false;
        containerAnimator.SetTrigger("throwSludges");
        //and then set moving to true at the end of the sludge throwing animation
    }

    void MeleeAttack() {
        //wind up and make an attack in the area around herself?
    }

    void FloodStage() {
        //disperse into sludge and flood the bottom of the stage
        moving = false;
        containerAnimator.SetTrigger("floodStage");
    }

    void SwitchSides() {
        //move to the other side of the stage
    }

    public override void StartFight() {
        //disable further player interactions
        GetComponent<BoxCollider2D>().enabled = false;
        if (!foughtBefore) {
            Intro();
        } else {
            fighting = true;
            anim.SetBool("fighting", true);
        }
    }

    void Intro() {
        this.foughtBefore = true;
        containerAnimator.SetTrigger("awake");
        uc.OpenDialogue(this);
        uc.RenderDialogue(monologue[0]);
    }

    void AddLines() {
        monologue.Add(new DialogueLine(
            "Half sludge, half light...",
            this.bossName,
            0
        ));
        monologue.Add(new DialogueLine(
            "The enviroment does terrible things to one down here.",
            this.bossName,
            0
        ));
        monologue.Add(new DialogueLine(
            "But not to you, yet.\n",
            this.bossName,
            0
        ));
        monologue.Add(new DialogueLine(
            "Are you here to kill? To maim? To take what you can from these wastes?",
            this.bossName,
            0
        ));
        monologue.Add(new DialogueLine(
            "...",
            "VAL",
            -1
        ));
        monologue.Add(new DialogueLine(
            "Get it over with, then.",
            this.bossName,
            0
        ));
    }

    public override void StopTalking() {
        playerObject.GetComponent<PlayerController>().UnFreezeInSpace();
        fighting = true;
        anim.SetBool("fighting", true);
        StartMoving();
        anim.speed = 1;
    }

    void CloseEye(int eyeNum) {
        eyeCount--;
        GameObject e = eyeContainer.transform.GetChild(eyeCount).gameObject;
        e.GetComponent<BoxCollider2D>().enabled = false;
        e.GetComponent<Animator>().SetTrigger("close");
    }

    public override void OnDamage() {
        //active eyes represent current health state
        float eyeFraction = (float) (eyeCount - 1) / (float) initialEyeCount;
        float healthFraction = (float) hp / (float) totalHP;
        if (eyeFraction >= healthFraction) {
            CloseEye(eyeCount - 1);
        }
        
    }

    public void LowerWalls() {

    }

    public void RaiseWalls() {
        
    }

    public void EndDialogue() {
        anim.speed = 0;
        AddEndLines();
        playerObject.GetComponent<PlayerController>().FreezeInSpace();
        uc.OpenDialogue(this);
        uc.RenderDialogue(monologue[0]);
    }

    void AddEndLines() {
        currentLine = 0;
        monologue.Clear();
        monologue.Add(new DialogueLine(
            "That's it! Thanks for playing!",
            this.bossName,
            0
        ));
        monologue.Add(new DialogueLine(
            "Excuse me while I die.",
            this.bossName,
            0
        ));
    }
     
}
