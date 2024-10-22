using System.Collections;
using System.Collections.Generic;
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

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Input Action Asset")]
    public InputActionAsset inputActionAsset; // Reference to the InputActionAsset

    private InputAction dashAction; // Define the dash action

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        // Initialize the dash input action from the InputActionAsset
        var playerActionMap = inputActionAsset.FindActionMap("Player");
        dashAction = playerActionMap.FindAction("Dash");

        // Enable the dash action and bind the Dash method to its performed event
        dashAction.Enable();
        dashAction.performed += ctx => Dash();
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

        // Reset cooldown timer
        dashCdTimer = dashCd;

        // Start dashing
        pm.dashing = true;

        // Calculate the force to apply during dash
        Vector3 forceToApply = orientation.forward * dashForce + orientation.up * dashUpwardForce;
        delayedForceToApply = forceToApply;

        // Apply the force with a slight delay
        Invoke(nameof(DelayedDashForce), 0.025f);

        // Stop dashing after the dash duration
        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        // Apply the dash force to the Rigidbody
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        // End the dashing state
        pm.dashing = false;
    }
}
