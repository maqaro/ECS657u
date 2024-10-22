using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // Import the Input System namespace

public class PlayerCam : MonoBehaviour
{
    [Header("Sensitivity")]
    public float senX;
    public float senY;

    [Header("References")]
    public Transform orientation;

    private float xRotation;
    private float yRotation;

    [Header("Input Action Asset")]
    public InputActionAsset inputActionAsset;  // Reference to the InputActionAsset

    private InputAction lookAction;  // Action for mouse movement

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Find the action map and actions
        var playerActionMap = inputActionAsset.FindActionMap("Player");
        lookAction = playerActionMap.FindAction("Look");

        // Enable the look action
        lookAction.Enable();
    }

    private void Update()
    {
        // Get mouse delta values from the new input system
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>();

        float mouseX = mouseDelta.x * Time.deltaTime * senX;
        float mouseY = mouseDelta.y * Time.deltaTime * senY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply rotations
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
