using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour {

    [SerializeField] GameObject pressXUI;
    [SerializeField] GameObject pressAUI;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TogglePressX() {
        // Only turn it on if the other UI is off
        if (!pressAUI.activeSelf) {
            //  If it's on turn it off
            if (pressXUI.activeSelf) {
                pressXUI.SetActive(false);
            }
            // If it is off turn it on
            else {
                pressXUI.SetActive(true);
            }
        }
    }

    public void ToggleOnPressA() {
        // Only turn it on if the other UI is off
        if (!pressXUI.activeSelf) {
            pressAUI.SetActive(true);
        }
    }

    public void ToggleOffPressA()
    {
        if (!pressAUI.activeSelf) {
            pressAUI.SetActive(false);
        }
    }
}
