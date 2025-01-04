using UnityEngine;
using UnityEngine.InputSystem;

public class RotateMirror : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private Contols controls;
    private bool rotateLeft;
    private bool rotateRight;
    private bool playerInRange = false;  // Track if the player is in the trigger zone

    private void OnEnable()
    {
        controls = new Contols();
        controls.Player.Enable();

        controls.Player.RotateMirrorLeft.performed += ctx => { if (playerInRange) rotateLeft = true; };
        controls.Player.RotateMirrorLeft.canceled += ctx => rotateLeft = false;

        controls.Player.RotateMirrorRight.performed += ctx => { if (playerInRange) rotateRight = true; };
        controls.Player.RotateMirrorRight.canceled += ctx => rotateRight = false;
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        if (!playerInRange) return;  // Only rotate when the player is in the trigger zone

        float direction = 0;
        if (rotateLeft) direction -= 1;
        if (rotateRight) direction += 1;

        transform.Rotate(Vector3.up * direction * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the entering object is the player
        {
            playerInRange = true;
            Debug.Log("Player entered range - can rotate mirror");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the exiting object is the player
        {
            playerInRange = false;
            rotateLeft = false;  // Stop rotation immediately when out of range
            rotateRight = false;
            Debug.Log("Player exited range - cannot rotate mirror");
        }
    }
}
