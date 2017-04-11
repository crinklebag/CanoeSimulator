using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelOne : MonoBehaviour {

    [SerializeField] GameObject[] waterPanels;
    [SerializeField] float lowWaterLevel = 0;
    [SerializeField] float midWaterLevel = 0;
    [SerializeField] float highWaterLevel = 0;
    [SerializeField] float raiseWaterLevelSpeed = 20;
    [SerializeField] GameObject playerCanoe;
    float currentWaterLevel = -5;

    [Header("Waterfall")]
    [SerializeField] GameObject[] waterfallLogs;
    [SerializeField] ParticleSystem[] splashes;

    [Header("Dam")]
    [SerializeField] GameObject[] trees;
    [SerializeField] GameObject swampMat;
    [SerializeField] GameObject dam;
    bool moveDam = false;
    bool raiseDam = false;
    float damXPos = 2;

    [Header("Water Bridge")]
    [SerializeField] GameObject current;
    [SerializeField] GameObject waterAnim;
    [SerializeField] GameObject walls;

    bool raiseWater;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (raiseWater) {
            RaiseWater();
        }
        if (raiseDam) {
            RaiseDam();
        }
        if (moveDam) {
            MoveDam();
        }
    }

    void RaiseWater() {
        foreach (GameObject waterPanel in waterPanels) {
            if (waterPanel.transform.position.y < -2.9) {
                waterPanel.transform.position = Vector3.Lerp(waterPanel.transform.position, new Vector3(waterPanel.transform.position.x, currentWaterLevel, waterPanel.transform.position.z), Time.deltaTime * raiseWaterLevelSpeed);
            }
        }
    }

    void RaiseDam() {
        dam.transform.localPosition = Vector3.Lerp(dam.transform.localPosition, new Vector3(dam.transform.localPosition.x, -0.2f, dam.transform.localPosition.z), Time.deltaTime * raiseWaterLevelSpeed);
    }

    void MoveDam() {
        swampMat.transform.localPosition = Vector3.Lerp(swampMat.transform.localPosition, new Vector3(damXPos, swampMat.transform.localPosition.y, swampMat.transform.localPosition.z), Time.deltaTime * raiseWaterLevelSpeed);
    }

    public void CheckWaterfallLogs() {
        int brokenLogs = 0;

        foreach (GameObject log in waterfallLogs) {
            if (log.GetComponent<BreakableObject>().IsBroken()) {
                brokenLogs++;
            }
        }

        if (brokenLogs == waterfallLogs.Length) {
            // Increase lifespan on the particle effects
            foreach (ParticleSystem splash in splashes) {
                var main = splash.main;
                main.startLifetime = 1.0f;
            }

            //Increase the height of all the water panels
            raiseWater = true;
            currentWaterLevel = midWaterLevel;

            // Increase the water level in water physics
            playerCanoe.GetComponent<RealisticBuoyancy>().waterLevelOverride = midWaterLevel;

            // turn on water currents - when they are in the level

        }
    }

    public void CheckDam() {
        int foundTrees = 0;

        foreach (GameObject tree in trees)
        {
            if (tree.GetComponent<BreakableObject>().IsBroken())
            {
                foundTrees++;
            }
        }

        // Move the dam out into the river
        switch (foundTrees) {
            case 1:
                moveDam = true;
                damXPos = 0;
                break;
            case 2:
                damXPos = -1;
                break;
            case 3:
                damXPos = -5;
                break;
        }

        if (foundTrees == trees.Length) {
            // raise the water 
            currentWaterLevel = highWaterLevel;
            // raise the dam
            raiseDam = true;
            // Raise the player UP
            playerCanoe.GetComponent<RealisticBuoyancy>().waterLevelOverride = highWaterLevel;
            // Turn on the current over the bridge and the animation
            waterAnim.SetActive(true);
            //Toggle all necessary colliders
            current.SetActive(true);
            walls.SetActive(false);
        }
    }

    public int GetFallenTrees() {
        int foundTrees = 0;

        foreach (GameObject tree in trees)
        {
            if (tree.GetComponent<BreakableObject>().IsBroken())
            {
                foundTrees++;
            }
        }

        return foundTrees;
    }
}
