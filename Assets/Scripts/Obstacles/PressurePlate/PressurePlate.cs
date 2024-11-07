using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject door;
    private MovingDoor doorScript;

    private void Start()
    {
        doorScript = door.GetComponent<MovingDoor>();
    }

    // When the player enters the trigger area, the door will open
    private void OnTriggerEnter(Collider other)
    {
        if (doorScript != null)
        {
            doorScript.OpenDoor();
        }
    }

    // When the player exits the trigger area, the door will close
    private void OnTriggerExit(Collider other)
    {
        if (doorScript != null)
        {
            doorScript.CloseDoor();
        }
    }
}
