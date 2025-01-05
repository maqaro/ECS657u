using UnityEngine;
using UnityEngine.InputSystem;

public class RotateMirror : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private Contols controls;
    private bool rotateLeft; // Sets the bind for the left rotation of the mirror
    private bool rotateRight; // Sets the bind for the right rotation of the mirror
    private bool playerInRange = false;  // Track if the player is in the trigger zone

    //Enables the player controls to rotate the 'mirror'
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
        if (rotateLeft) direction -= 1; // rotating left sets it in the -1 direction
        if (rotateRight) direction += 1; // rotating right sets it in the 1 direction

        transform.Rotate(Vector3.up * direction * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the entering object is the player
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if the exiting object is the player
        {
            playerInRange = false;
            rotateLeft = false;  // Stop ability to rotate once the player is out of the range
            rotateRight = false;
        }
    }
}
