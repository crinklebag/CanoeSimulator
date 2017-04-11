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

    public void ToggleOnPressX()
    {
        // Only turn it on if the other UI is off
        // Debug.Log("Turning A On");
        if (!pressXUI.activeSelf) { pressXUI.SetActive(true); }
    }

    public void ToggleOffPressX()
    {
        // Debug.Log("Turning A Off");
        if (pressXUI.activeSelf) { pressXUI.SetActive(false); }
    }

    public void ToggleOnPressA() {
        // Only turn it on if the other UI is off
        // Debug.Log("Turning A On");
        if (!pressAUI.activeSelf) { pressAUI.SetActive(true); }
    }

    public void ToggleOffPressA()
    {
        // Debug.Log("Turning A Off");
        if (pressAUI.activeSelf) { pressAUI.SetActive(false); }
    }
}
