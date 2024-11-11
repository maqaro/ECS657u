using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    public Transform cameraPosition;

    void Update()
    {
        // Move the camera to the player's position
        transform.position = cameraPosition.position;
    }
}
