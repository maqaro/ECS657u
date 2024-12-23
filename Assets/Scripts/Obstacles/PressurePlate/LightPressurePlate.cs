using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPressurePlate : MonoBehaviour
{
    [Header("References")]
    public GameObject door;
    private MovingDoor doorScript;

    [Header("Layer Mask for Light Beam")]
    [SerializeField] private LayerMask lightrayLayer;

    private bool lightBeamActive = false;

    private void Start()
    {
        doorScript = door.GetComponent<MovingDoor>();

        // If the reference is missing, log an error.
        if (doorScript == null)
        {
            Debug.LogError("MovingDoor script not found on door object!");
        }
    }

    private void Update()
    {
        CheckLightBeam();
    }

    public void CheckLightBeam()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * 100, Color.red);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, lightrayLayer))
        {
            Debug.Log($"Raycast hit: {hit.collider.name} on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            if (hit.collider.CompareTag("PressurePlate"))
            {
                Debug.Log("Light beam is hitting the pressure plate.");

                if (!lightBeamActive)
                {
                    // Light beam is now hitting the plate, so open the door.
                    doorScript.OpenDoor();
                    Debug.Log("Door opened.");
                    lightBeamActive = true; // Mark the beam as active.
                }
            }
            else
            {
                Debug.LogWarning($"Raycast hit an object but it's not the pressure plate: {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");

            if (lightBeamActive)
            {
                doorScript.CloseDoor();
                Debug.Log("Door closed.");
                lightBeamActive = false; // Mark the beam as inactive.
            }
        }
    }
}
