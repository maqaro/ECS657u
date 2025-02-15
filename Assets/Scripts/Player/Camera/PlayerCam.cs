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
    public Transform Arms;
    public MainMenu mm;

    [Header("Input Action Asset")]
    public InputActionAsset inputActionAsset;  

    private InputAction lookAction;

    private void Start()
    {
        // Hide and lock the cursor
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
        if (lookAction.enabled){
        // Get mouse delta values from the new input system
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>();

        float mouseX = mouseDelta.x * Time.deltaTime * senX * mm.mainSensitivity;
        float mouseY = mouseDelta.y * Time.deltaTime * senY * mm.mainSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Arms.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Apply rotations
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        
    
    }

    public void EnableCameraMovement(){ //Enable camera movement
            lookAction.Enable();
        }

    public void DisableCameraMovement(){ //Disable camera movement (for dialogue)
        lookAction.Disable();
    }


}