﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MyMonoBehaviour, IObserver
{
    public PlayerShip PlayerShip { get; set; }

    Animator anim;
    CanvasGroup canvasGroup;


    // Start is called before the first frame update
    void Awake() {
		Assert.IsNotNull(PlayerShip);

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerShip.runData.raceFinished)
            anim.SetTrigger("RaceFinished");
    }

    public void OnNotify(float score, float charges)
    {
        // Do Something
        
    }
}
