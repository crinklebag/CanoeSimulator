using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BreakableObject : MonoBehaviour {

    LevelOne gc;

    [SerializeField] GameObject brokenObject;
    [SerializeField] GameObject unbrokenObject;
    [SerializeField] bool destroyable = true;
    bool broken = false;

    [SerializeField] bool lastObject = false;

	// Use this for initialization
	void Start () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelOne>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BreakObject() {
        // Debug.Log("Break Object");
        // Check to see if the count in the game controller is 2 - if it is then this tree can be broken
        if (!lastObject || gc.GetFallenTrees() == 2)
        {
            GameObject brokenPieces = Instantiate(brokenObject, this.transform.position, this.transform.rotation) as GameObject;
            RealisticBuoyancy[] pieces = brokenPieces.GetComponentsInChildren<RealisticBuoyancy>();

            if (pieces != null)
            {
                foreach (RealisticBuoyancy buoyancyObject in pieces)
                {
                    // use the reference to set up the buoyancy of the object
                    buoyancyObject.GetComponent<RealisticBuoyancy>().setup();
                    // Fix the water level
                    buoyancyObject.GetComponent<RealisticBuoyancy>().waterLevelOverride = RealisticWaterPhysics.currentWaterLevel;
                    if (destroyable) { Destroy(buoyancyObject.gameObject, 1.0f); }
                }
            }

            Destroy(unbrokenObject);
            broken = true;
            gc.GetComponent<LevelOne>().CheckWaterfallLogs();
            gc.GetComponent<LevelOne>().CheckDam();

        } 
    }

    public bool IsBroken() {
        return broken;
    }
}
