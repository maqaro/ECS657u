using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform PlayerCam;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVal = true;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Input Action Asset")]
    public InputActionAsset inputActionAsset; // Reference to the InputActionAsset

    private InputAction dashAction; // Define the dash action
    private InputAction moveAction; // Define the move action
    private Vector2 moveInput;

    void Start()
    {
        // Get the Rigidbody and PlayerMovement components
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        // Initialize the dash input action from the InputActionAsset
        var playerActionMap = inputActionAsset.FindActionMap("Player");
        dashAction = playerActionMap.FindAction("Dash");
        moveAction = playerActionMap.FindAction("Move");

        // Enable the dash action and bind the Dash method to its performed event
        dashAction.Enable();
        dashAction.performed += ctx => Dash();

        moveAction.Enable();
    }

    void Update()
    {
        // Handle the cooldown timer
        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        // If dash is still on cooldown, return
        if (dashCdTimer > 0)
            return;
        else
            dashCdTimer = dashCd;

        // Start dashing
        pm.dashing = true;

        Transform forwardT;

        if (useCameraForward)
            forwardT = PlayerCam;
        else
            forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        // Calculate the force to apply during dash
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
        
        if (disableGravity)
            rb.useGravity = false;
        
        delayedForceToApply = forceToApply;

        // Apply the force with a slight delay
        Invoke(nameof(DelayedDashForce), 0.025f);

        // Stop dashing after the dash duration
        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {

        if (resetVal)
            rb.velocity = Vector3.zero;

        // Apply the dash force to the Rigidbody
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        // End the dashing state
        pm.dashing = false;

        if (disableGravity)
            rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        // Read the movement input from the new Input System
        moveInput = moveAction.ReadValue<Vector2>();

        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y;

        Vector3 direction = Vector3.zero;

        if (allowAllDirections)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }



}