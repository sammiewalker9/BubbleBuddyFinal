using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Fish : MonoBehaviour
{

    private const float JUMP_AMOUNT = 0.6f;

    private static Fish instance; 
    public static Fish GetInstance()
    {
        return instance;
    }

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;


    private Rigidbody2D fishrigidbody2D;
    private State state;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead,
    }

    private void Awake()
    {

        instance = this;
        fishrigidbody2D = GetComponent < Rigidbody2D>();
        fishrigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;

    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    fishrigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump(); 
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump(); 
                }
                break;
            case State.Dead:
                break;

        }


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Jump();
        }
    }

    private void Jump()
    {
        fishrigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
         SoundManager.PlaySound(SoundManager.Sound.FishJump );
    }

    private void OnTriggerEnter2D(Collider2D collider)
    { 
        fishrigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);

        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
}
