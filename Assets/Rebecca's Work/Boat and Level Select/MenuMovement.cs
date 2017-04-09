﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class MenuMovement : MonoBehaviour {
    
    [SerializeField] InstructionPanel instructionsPanel;

    [Header("Boat Selection Variables")]
    [SerializeField] Image[] boatUI;
    [SerializeField] GameObject boatSelectedUI;
    [SerializeField] GameObject pressAUI;
    [SerializeField] GameObject pressBUI;
    [SerializeField] GameObject[] boatBody;
    [SerializeField] GameObject boatContents;

    [Header("Movement Variables")]
    [SerializeField] GameObject playerCharacter;
    [SerializeField] GameObject paddlePivot;
    [SerializeField] GameObject paddle;
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
    [SerializeField] float splashBackDelay = 0.5f;
    [SerializeField] float splashForwardDelay = 0.25f;

    Player player;
    GameObject boat;
    Quaternion initRot;
    string selectedBoat = "canoe";
    int dir = 0;
    int previousPaddleSide;
    int previousPaddleDirection;
    float paddleRotationTimer = 0;
    bool canAttack = false;
    bool canPaddle = true;
    bool canInput = true;
    bool selectingBoat = true;

    // reference to the last player found within reach
    GameObject foundPlayer = null;

    //creating an selectable object.
    [SerializeField]  GameObject attackDisplay;
    Collider[] hitColliders;

    // Use this for initialization
    void Awake() {
        player = ReInput.players.GetPlayer(playerID);
        boat = this.gameObject;
        if (paddle != null) initRot = paddle.transform.localRotation;
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

        // Mover Camera Up
        if (player.GetAxisRaw("Vertical") > 0) {

            Camera.main.GetComponent<SmoothFollowCSharp>().RotateUp();

        } else if (player.GetAxisRaw("Vertical") < 0) {
            Camera.main.GetComponent<SmoothFollowCSharp>().RotateDown();
        }

        RotatePaddle();
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

    void CanAttack()
    {
        attackDisplay.SetActive(false);

        hitColliders = Physics.OverlapSphere(GetPaddlePosition(), paddleDataCanoe.reach);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject != this.gameObject && hitColliders[i].GetComponent<MenuBoat>())
            {
                attackDisplay.SetActive(true);
                foundPlayer = hitColliders[i].gameObject;
                Debug.Log("Can Attack");
            }
        }

        // Check now to see if the attack notice is on or off - if it is on turn on the other players attack radius
        if (attackDisplay.activeSelf)
        {
            // If the current state is end(3) or off(0), activate
            if (foundPlayer != null && (foundPlayer.GetComponent<PlayerAttackUIController>().GetState() == 0 || foundPlayer.GetComponent<PlayerAttackUIController>().GetState() == 3))
            {
                foundPlayer.GetComponent<PlayerAttackUIController>().ActivateRadius();
                Debug.Log("Activate The Attack UI");
            }
        }
        else
        {
            // If the current state is start(1) or pulse(2), end it
            if (foundPlayer != null && (foundPlayer.GetComponent<PlayerAttackUIController>().GetState() == 1 || foundPlayer.GetComponent<PlayerAttackUIController>().GetState() == 2))
            {
                foundPlayer.GetComponent<PlayerAttackUIController>().DeactivateRadius();
                Debug.Log("Deactivate The Attack UI");
            }
        }

    }

    Vector3 GetPaddlePosition()
    {
        Bounds paddleBounds = paddle.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;

        return paddle.transform.position - paddle.transform.up.normalized * paddleBounds.extents.z * 0.33f;
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

    public void AllowAttack() {
        canAttack = true;
    }

}