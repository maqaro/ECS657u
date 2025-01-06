using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI; // Required for Image components

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

    [Header("Cooldown UI")]
    public Image cooldownImage; // Reference to the grey cooldown overlay

    [Header("Input Action Asset")]
    public InputActionAsset inputActionAsset;

    private InputAction dashAction;
    private InputAction moveAction;
    private Vector2 moveInput;

    [SerializeField] private AudioClip dashSound; // Sound to play when dashing

    private bool isAirDashing;
    private DialogueUI dialogueUI; // Reference to DialogueUI

    // Initializes references and sets up input bindings
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

        // Initialize the cooldown image
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // Start empty
        }

        // Find the DialogueUI in the scene
        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    // Handles cooldown for dashing
    void Update()
    {
        // Update the cooldown timer
        if (dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;

            // Update the UI overlay if it exists
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = dashCdTimer / dashCd;
            }
        }
    }

    // Cleans up input bindings when the script is disabled
    void OnDisable()
    {
        if (dashAction != null) dashAction.performed -= ctx => Dash();
    }

    // Executes the dash action when triggered, ensuring dashing is disabled during dialogue
    private void Dash()
    {
        // Prevent dashing if dialogue is active
        if (dialogueUI != null && dialogueUI.IsDialogueActive)
        {
            return;
        }

        if (dashCdTimer > 0 || (isAirDashing && !pm.grounded) || pm == null || rb == null || orientation == null || PlayerCam == null){return;}

        SoundFXManager.instance.PlaySfxPlayer(dashSound, transform, 0.3f);
        dashCdTimer = dashCd; // Start the cooldown
        pm.dashing = true;
        isAirDashing = !pm.grounded;

        pm.enabled = false;

        Transform forwardT = useCameraForward ? PlayerCam : orientation;
        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;

        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);

        // Reset the UI overlay for cooldown
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f; // Set to full
        }
    }

    private Vector3 delayedForceToApply;

    // Applies the dash force after a slight delay
    private void DelayedDashForce()
    {
        if (resetVal)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    // Resets the dash state and smoothly reduces velocity
    private void ResetDash()
    {
        pm.dashing = false;

        if (disableGravity)
            rb.useGravity = true;

        // Smoothly reduce velocity to avoid abrupt stopping
        Vector3 currentVelocity = rb.velocity;
        DOTween.To(() => currentVelocity, x => currentVelocity = x, Vector3.zero, 0.3f)
            .OnUpdate(() =>
            {
                rb.velocity = currentVelocity; // Apply the tweened velocity
            });

        // Reset air dash state and restore PlayerMovement
        isAirDashing = false;
        pm.enabled = true;
    }

    // Calculates the direction for the dash based on input
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
