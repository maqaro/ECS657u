using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPressurePlate : MonoBehaviour
{
    [Header("References")]
    public GameObject door; // Reference to the door GameObject
    private MovingDoor doorScript;

    private bool lightBeamActive = false;

    private void Start()
    {
        doorScript = door.GetComponent<MovingDoor>();

        // If the reference is missing, ensure the game doesn't break.
        if (doorScript == null)
        {
            return;
        }
    }

    public void CheckLightBeam()
    {
        // Only trigger if the light beam state changes
        if (!lightBeamActive)
        {
            doorScript.OpenDoor();
            lightBeamActive = true;
        }
    }

    private void Update()
    {
        // Optionally check if the light beam is no longer hitting the plate
        if (lightBeamActive)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, Mathf.Infinity) || !hit.collider.CompareTag("PressurePlate"))
            {
                doorScript.CloseDoor();
                lightBeamActive = false;
            }
        }
    }
}
