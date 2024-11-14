using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoor : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public float speed = 1.0f;
    private Vector3 leftDoorStart;
    private Vector3 rightDoorStart;
    private Vector3 leftDoorEnd;
    private Vector3 rightDoorEnd;
    public bool isOpen = false;
    private Coroutine leftDoorCoroutine;
    private Coroutine rightDoorCoroutine;

    private void Start()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("Left or Right door is not assigned.");
            return;
        }

        leftDoorStart = leftDoor.transform.position;
        rightDoorStart = rightDoor.transform.position;
        leftDoorEnd = leftDoorStart + Vector3.forward * 5.0f;
        rightDoorEnd = rightDoorStart + Vector3.back * 5.0f;
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            // Stop any existing coroutines first
            if (leftDoorCoroutine != null) StopCoroutine(leftDoorCoroutine);
            if (rightDoorCoroutine != null) StopCoroutine(rightDoorCoroutine);
            
            leftDoorCoroutine = StartCoroutine(MoveDoor(leftDoor, leftDoorEnd));
            rightDoorCoroutine = StartCoroutine(MoveDoor(rightDoor, rightDoorEnd));
            isOpen = true;
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            // Stop any existing coroutines first
            if (leftDoorCoroutine != null) StopCoroutine(leftDoorCoroutine);
            if (rightDoorCoroutine != null) StopCoroutine(rightDoorCoroutine);
            
            leftDoorCoroutine = StartCoroutine(MoveDoor(leftDoor, leftDoorStart));
            rightDoorCoroutine = StartCoroutine(MoveDoor(rightDoor, rightDoorStart));
            isOpen = false;
        }
    }

    private IEnumerator MoveDoor(GameObject door, Vector3 target)
    {
        while (Vector3.Distance(door.transform.position, target) > 0.01f)
        {
            door.transform.position = Vector3.MoveTowards(door.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        door.transform.position = target; // Ensure door reaches exact position
    }
}
