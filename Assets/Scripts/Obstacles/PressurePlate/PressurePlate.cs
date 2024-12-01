using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("References")]
    public GameObject door;
    private MovingDoor doorScript;

    [Header("Tags")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string pickupTag = "canPickUp";

    private void Start()
    {
        doorScript = door.GetComponent<MovingDoor>();
    }

    // When the player enters the trigger area, the door will open
    private void OnTriggerEnter(Collider other)
    {
        if (doorScript != null && (other.CompareTag(playerTag) || other.CompareTag(pickupTag)))
        {
            doorScript.OpenDoor();
        }
    }

    // When the player exits the trigger area, the door will close
    private void OnTriggerExit(Collider other)
    {
        if (doorScript != null && (other.CompareTag(playerTag) || other.CompareTag(pickupTag)))
        {
            doorScript.CloseDoor();
        }
    }
}
