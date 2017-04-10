using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MenuBoat : MonoBehaviour {
    


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Fix the water level
            this.GetComponent<RealisticBuoyancy>().waterLevelOverride = 0;
        }
	}

	void OnCollisionEnter(Collision other){

		// Debug.Log ("Colliding with: " + other);

		if (other.gameObject.CompareTag ("Player")) {
			// Debug.Log ("Players Hit");
			this.GetComponent<AudioSource> ().Play ();
			this.GetComponentInParent<MenuMovement> ().RumbleControllers ();
			other.gameObject.GetComponentInParent<MenuMovement> ().RumbleControllers ();
		}
	}
}
