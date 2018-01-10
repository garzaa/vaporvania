using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyOfTheLake : Boss {

    int eyeCount;
    public GameObject eyeContainer;

    void Initialize() {
        monologue = new List<DialogueLine>();
        AddLines();
        eyeCount = eyeContainer.transform.childCount;
    }


    public override void BossMove() {
        //maybe check for one of a few actions to take?
        if (!fighting) return;
    }

    void SpawnSludges() {
        //throw three sludges at the player
    }

    void MeleeAttack() {
        //wind up and make an attack in the area around herself
    }

    void FloodStage() {
        //disperse into sludge and flood the bottom of the stage
    }

    public override void StartFight() {
        if (!foughtBefore) {
            Intro();
        } else {
            fighting = true;
            anim.SetBool("fighting", true);
        }
    }

    void Intro() {

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
            "But not to you, yet.\n" +
            "Are you here to kill? To maim? To take what you can from these wastes?",
            this.bossName,
            0
        ));
        monologue.Add(new DialogueLine(
            "Let's get it over with, then.",
            this.bossName,
            1
        ));
    }

    public override void StopTalking() {
        StartFight();
    }

    void CloseEye(int eyeNum) {
        GameObject e = eyeContainer.transform.GetChild(eyeCount).gameObject;
        e.GetComponent<BoxCollider2D>().enabled = false;
        e.GetComponent<SpriteRenderer>().enabled = false;
    }
}
