using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MenuBoat : MonoBehaviour {

    PlayerUIController uiController;

	// Use this for initialization
	void Start () {
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerUIController>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

	void OnCollisionEnter(Collision other){

		// Debug.Log ("Colliding with: " + other);

		if (other.gameObject.CompareTag ("Massive Log")) {
            Debug.Log("Hit Massive Log");
            other.gameObject.GetComponentInParent<BreakableObject>().BreakObject();
            uiController.PlayWin();
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boost Current")) {
            this.GetComponent<MenuMovement>().SetCanBoost(true);
            // uiController.ToggleOnPressX();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boost Current")) {
            this.GetComponent<MenuMovement>().SetCanBoost(false);
            // uiController.ToggleOffPressX();
        }
    }
}
