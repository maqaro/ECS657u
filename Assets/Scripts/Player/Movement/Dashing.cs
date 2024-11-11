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
    public InputActionAsset inputActionAsset;

    private InputAction dashAction;
    private InputAction moveAction;
    private Vector2 moveInput;

    private bool isAirDashing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        var playerActionMap = inputActionAsset.FindActionMap("Player");
        dashAction = playerActionMap.FindAction("Dash");
        moveAction = playerActionMap.FindAction("Move");

        dashAction.Enable();
        dashAction.performed += ctx => Dash();

        moveAction.Enable();
    }

    void Update()
    {
        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        if (dashCdTimer > 0 || (isAirDashing && !pm.grounded))
            return;

        dashCdTimer = dashCd;
        pm.dashing = true;
        isAirDashing = !pm.grounded;

        // Temporarily disable movement speed control
        pm.enabled = false;

        Transform forwardT = useCameraForward ? PlayerCam : orientation;
        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;

        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        if (resetVal)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;

        if (disableGravity)
            rb.useGravity = true;

        // Reset air dash state and restore `PlayerMovement`
        isAirDashing = false;
        rb.velocity = Vector3.zero;
        pm.enabled = true; // Re-enable `PlayerMovement`
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        moveInput = moveAction.ReadValue<Vector2>();

        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y;

        Vector3 direction = allowAllDirections
            ? forwardT.forward * verticalInput + forwardT.right * horizontalInput
            : forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }
}
