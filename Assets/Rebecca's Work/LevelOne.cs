using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOne : MonoBehaviour {
    
    [SerializeField] GameObject[] waterPanels;
    [SerializeField] float lowWaterLevel = 0;
    [SerializeField] float highWaterLevel = 0;
    [SerializeField] float raiseWaterLevelSpeed = 20;
    [SerializeField] GameObject playerCanoe;

    [Header("Waterfall")]
    [SerializeField] GameObject[] waterfallLogs;
    [SerializeField] ParticleSystem[] splashes;

    bool raiseWater;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (raiseWater) {
            RaiseWater();
        }
	}

    void RaiseWater() {
        foreach (GameObject waterPanel in waterPanels) {
            if (waterPanel.transform.position.y < -2.9) {
                waterPanel.transform.position = Vector3.Lerp(waterPanel.transform.position, new Vector3(waterPanel.transform.position.x, highWaterLevel, waterPanel.transform.position.z), Time.deltaTime * raiseWaterLevelSpeed);
            }
        }
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

            // Increase the water level in water physics
            playerCanoe.GetComponent<RealisticBuoyancy>().waterLevelOverride = highWaterLevel;

            // turn on water currents - when they are in the level

        }
    }
}
