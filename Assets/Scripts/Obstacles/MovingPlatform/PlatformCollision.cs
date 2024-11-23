using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] Transform platform;

    private void OnTriggerEnter(Collider other)
    {
        // If the player enters the platform, set the platform as the parent
        if (other.gameObject.tag.Equals(playerTag))
        {
            other.gameObject.transform.parent = platform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the player exits the platform, remove the platform as the parent
        if (other.gameObject.tag.Equals(playerTag))
        {
            other.gameObject.transform.parent = null;
        }
    }
}
