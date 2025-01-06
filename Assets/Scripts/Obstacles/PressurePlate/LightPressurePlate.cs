using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPressurePlate : MonoBehaviour
{
    [Header("References")]
    public GameObject door; // Reference to the door GameObject
    private MovingDoor doorScript;

    private bool lightBeamActive = false; // Tracks if the light beam is active
    private bool stateChanged = false;   // Tracks if the door state has recently changed

    private void Start()
    {
        // Get the MovingDoor script from the door GameObject
        doorScript = door.GetComponent<MovingDoor>();
    }

    public void CheckLightBeam()
    {
        if (!lightBeamActive)
        {
            // Open the door if the light beam activates the pressure plate
            doorScript.OpenDoor();
            lightBeamActive = true;
            stateChanged = true;
        }
    }

    private void Update()
    {
        // If the light beam is active, check if it is still hitting the plate
        if (lightBeamActive)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, Mathf.Infinity) || !hit.collider.CompareTag("PressurePlate"))
            {
                // Close the door if the light beam is no longer hitting the plate
                if (!stateChanged)
                {
                    doorScript.CloseDoor();
                    lightBeamActive = false;
                    stateChanged = true;
                }
            }
            else
            {
                // Reset stateChanged if the beam is still hitting
                stateChanged = false;
            }
        }
    }
}