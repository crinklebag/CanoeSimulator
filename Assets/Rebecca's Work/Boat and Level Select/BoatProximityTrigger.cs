using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatProximityTrigger : MonoBehaviour {

    PlayerUIController uiController;

    [SerializeField] string side;

    GameObject foundInteractable;
    bool canBreak;

	// Use this for initialization
	void Start () {
		uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PlayerUIController>();
	}
	
	// Update is called once per frame
	void Update () {

        // if the player is attacking and is still within reach - break the other object
        if (foundInteractable != null)
        {
            if (canBreak && this.GetComponentInParent<MenuMovement>().IsAttacking() && !foundInteractable.GetComponentInParent<BreakableObject>().IsBroken())
            {
                foundInteractable.GetComponentInParent<BreakableObject>().BreakObject();
                canBreak = false;
                StopAllCoroutines();
                StartCoroutine(BreakDelay());
            }
        }

        if (foundInteractable != null && foundInteractable.GetComponentInParent<BreakableObject>().IsBroken()) {
            // Turn off UI
            uiController.ToggleOffPressA();
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weak Log") && !other.GetComponentInParent<BreakableObject>().IsBroken())
        {
            canBreak = true;
        }
    }

    void OnTriggerStay(Collider other) {
         if (other.CompareTag("Weak Log") && !other.GetComponentInParent<BreakableObject>().IsBroken()) {
            Debug.Log("Hit Log");
            uiController.ToggleOnPressA();
            foundInteractable = other.gameObject;
            this.GetComponentInParent<MenuMovement>().AllowAttack(side);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Weak Log") && !other.GetComponentInParent<BreakableObject>().IsBroken()) {
            // Turn Off UI
            foundInteractable = null;
            uiController.ToggleOffPressA();
            canBreak = false;
        }
    }

    IEnumerator BreakDelay() {
        yield return new WaitForSeconds(1.5f);

        canBreak = true;
    }
}
