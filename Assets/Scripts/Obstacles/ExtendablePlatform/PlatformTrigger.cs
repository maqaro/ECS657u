using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    // List of platforms to be controlled by this trigger
    public List<GameObject> platforms;
    private List<ExtendablePlatform> platformScripts;

    private void Start()
    {
        platformScripts = new List<ExtendablePlatform>();

        // Get the ExtendablePlatform component from each platform
        foreach (var platform in platforms)
        {
            var platformScript = platform.GetComponent<ExtendablePlatform>();
            if (platformScript != null)
            {
                platformScripts.Add(platformScript);
            }
        }
    }

    // When the player enters the trigger area, extend all platforms
    private void OnTriggerEnter(Collider other)
    {
        foreach (var platformScript in platformScripts)
        {
            platformScript.ExtendPlatform();
        }
    }

    // When the player exits the trigger area, retract all platforms
    private void OnTriggerExit(Collider other)
    {
        foreach (var platformScript in platformScripts)
        {
            platformScript.RetractPlatform();
        }
    }
}
