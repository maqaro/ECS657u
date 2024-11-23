using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendablePlatform : MonoBehaviour
{
    public GameObject extendablePlatform;
    public float speed = 1.0f;
    public float distance;
    public Vector3 direction = Vector3.forward; // Direction can be set in the inspector

    private Vector3 extendablePlatformStart;
    private Vector3 extendablePlatformEnd;
    public bool isOpen = false;
    private Coroutine extendablePlatformCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if (extendablePlatform == null)
        {
            Debug.LogError("Extendable platform is not assigned.");
            return;
        }

        extendablePlatformStart = extendablePlatform.transform.position;
        extendablePlatformEnd = extendablePlatformStart + direction.normalized * distance;
    }

    public void ExtendPlatform()
    {
        if (!isOpen)
        {
            // Stop any existing coroutines first
            if (extendablePlatformCoroutine != null) StopCoroutine(extendablePlatformCoroutine);
            
            extendablePlatformCoroutine = StartCoroutine(MovePlatform(extendablePlatform, extendablePlatformEnd));
            isOpen = true;
        }
    }

    public void RetractPlatform()
    {
        if (isOpen)
        {
            // Stop any existing coroutines first
            if (extendablePlatformCoroutine != null) StopCoroutine(extendablePlatformCoroutine);
            
            extendablePlatformCoroutine = StartCoroutine(MovePlatform(extendablePlatform, extendablePlatformStart));
            isOpen = false;
        }
    }

    public void TogglePlatform()
    {
        if (isOpen)
        {
            RetractPlatform();
        }
        else
        {
            ExtendPlatform();
        }
    }

    private IEnumerator MovePlatform(GameObject platform, Vector3 target)
    {
        while (Vector3.Distance(platform.transform.position, target) > 0.01f)
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }
}
