﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class MenuMovement : MonoBehaviour {
    
    [SerializeField] InstructionPanel instructionsPanel;

    [Header("Movement Variables")]
    [SerializeField] GameObject playerCharacter;
    [SerializeField] int playerID;
    [SerializeField] PaddleData paddleDataRaft;
    [SerializeField] PaddleData paddleDataCanoe;
    float paddleForwardForce = 400;
    float paddleTorque = 200;
    float paddleRoationSpeed = 2400;

    [Header("Effects (Audio and Particles)")]
    [SerializeField] AudioSource splash;
    [SerializeField] AudioSource boatHit;
    [SerializeField] ParticleSystem splashForwardParticles;
    [SerializeField] ParticleSystem splashBackwardParticles;
    [SerializeField] ParticleSystem dustParticle;
    [SerializeField] ParticleSystem boostParticle;
    [SerializeField] float splashBackDelay = 0.5f;
    [SerializeField] float splashForwardDelay = 0.25f;

    [SerializeField]
    float speedBoostForce = 2000f;

    Player player;
    GameObject boat;
    string selectedBoat = "canoe";
    int dir = 0;
    int previousPaddleSide;
    int previousPaddleDirection;
    float paddleRotationTimer = 0;
    string triggerSide = "";
    bool canAttack = true;
    bool canPaddle = true;
    bool canInput = true;
    bool selectingBoat = true;
    bool attacking = false;
    bool boosting = false;
    bool canBoost = false;

    // reference to the last player found within reach
    GameObject foundPlayer = null;
    
    // Use this for initialization
    void Awake() {
        player = ReInput.players.GetPlayer(playerID);
        boat = this.gameObject;
    }

    // Update is called once per frame
    void Update() {
        if (player.GetButtonDown("+Right Paddle") && canPaddle) {
            Debug.Log("Pressed Button");
            MoveCanoe(1, -1);
        }
        else if (player.GetButtonDown("-Right Paddle") && canPaddle) {
            MoveCanoe(1, 1);
        }
        else if (player.GetButtonDown("+Left Paddle") && canPaddle) {
            MoveCanoe(-1, -1); 
        }
        else if (player.GetButtonDown("-Left Paddle") && canPaddle) {
            MoveCanoe(-1, 1);
        }
        else {
            // Don't move
            previousPaddleDirection = 0;
        }

        // Hit the powerup button && the boat has a powerup active
        if (canBoost && !boostParticle.isPlaying) {
            boostParticle.Play();
        }
        else if (canBoost && boostParticle.isPlaying) {
            boostParticle.Stop();
        }

        if (playerCharacter) {
            Animator playerAnimator = playerCharacter.GetComponent<Animator>();
            if (playerAnimator)
            {
                playerAnimator.SetBool("Stunned", canBoost);
            }
        }

        // Check to see if player is attacking
        Attack();
        RotatePaddle();
    }

    void Attack()
    {
        // Check if attacking
        if (player.GetButtonDown("Attack") && !attacking)
        {
            if (triggerSide == "right") { SetPaddleSide(1); }
            else if (triggerSide == "left") { SetPaddleSide(0); }

            // Check Wich side the player is on and switch it if need be
            playerCharacter.GetComponent<Animator>().SetTrigger("Attacking");

            // Set Attacking so the paddle knows to play particles and audio
            StopCoroutine(AttackDelay());
            attacking = true;
            StartCoroutine(AttackDelay());
        }
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.5f);

        // Play Audio and particle effects on the player paddle
        dustParticle.Play();
        boatHit.Play();

        // yield return new WaitForSeconds(0.2f);

        attacking = false;
    }

    void MoveCanoe(int paddleSide, int paddleDirection)
    {
        // Add force to boat by the paddle
        // Debug.Log("Adding Forward Force");
        canPaddle = false;
        
        Vector3 finalForwardForce = paddleDirection * paddleDataCanoe.forwardForce * boat.transform.forward;
        boat.transform.GetComponentInChildren<Rigidbody>().AddForceAtPosition(finalForwardForce, boat.transform.position, ForceMode.Impulse);

        Vector3 finalHorizontalForce = -paddleSide * paddleDataCanoe.torque * boat.transform.up;
        boat.transform.GetComponentInChildren<Rigidbody>().AddTorque(finalHorizontalForce, ForceMode.Impulse);

        previousPaddleSide = paddleSide;
        previousPaddleDirection = paddleDirection;

        if (playerCharacter)
        {
            // Debug.Log("Found Player");
            Animator playerAnimator = playerCharacter.GetComponent<Animator>();

            if (playerAnimator)
            {
                playerAnimator.SetInteger("Paddle Side", previousPaddleSide);
                if (paddleDirection > 0)
                {
                    playerAnimator.SetTrigger("Paddle Forward");
                    // Play Sound Effect
                    splash.Play();
                    // Play Splash Effect
                    StartCoroutine(PlaySplash(splashBackwardParticles, splashBackDelay));

                }
                else if (paddleDirection < 0)
                {
                    playerAnimator.SetTrigger("Paddle Backward");
                    // Play Sound Effect
                    splash.Play();
                    // Play Splash Effect
                    StartCoroutine(PlaySplash(splashForwardParticles, splashForwardDelay));
                }
            }
        }
    }

    void SetPaddleSide(int side)
    {
        previousPaddleSide = side;

        previousPaddleSide = side;

        if (playerCharacter)
        {
            Animator animator = playerCharacter.GetComponent<Animator>();
            if (animator)
            {
                animator.SetInteger("Paddle Side", side);
            }
        }
    }

    void RotatePaddle()
    {
        if (!canPaddle)
        {
            paddleRotationTimer += Time.deltaTime;
            
            if (paddleRotationTimer >= paddleDataCanoe.rotationTime)
            {
                canPaddle = true;
                paddleRotationTimer = 0;
            }
        }
    }

    IEnumerator WaitForInput() {

        canInput = false;

        yield return new WaitForSeconds(0.3f);

        canInput = true;

    }

    public int GetPlayerID() {
        return playerID;
    }

    IEnumerator PlaySplash(ParticleSystem splash, float delay) {

        yield return new WaitForSeconds(delay);
        splash.Play();
    }

    public void RumbleControllers() {
        StartCoroutine(Bump(0.1f));
    }

    // Variable length low-intensity bump function
    public IEnumerator Bump(float duration)
    {
        foreach (Joystick j in player.controllers.Joysticks)
        {
            if (!j.supportsVibration) continue;
            j.SetVibration(0.25f, 0.25f);
        }
        yield return new WaitForSeconds(duration);
        foreach (Joystick j in player.controllers.Joysticks)
        {
            j.StopVibration();
        }
    }

    public void AllowAttack(string side) {
        triggerSide = side;
    }

    public bool IsAttacking() {
        return attacking;
    }

    public void SetCanBoost(bool state) {
        canBoost = state;
    }

    public bool IsBoosting() {
        return boosting;
    }
}
