using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public float wallRunSpeed = 15f;
    public float wallRunJumpForce = 10f;
    public float wallGravityForce = 5f;
    public float wallRunTimeout = 1f;

    [Header("Detection")]
    public float wallCheckDistance = 1f;
    public float maxWallRunAngle = 60f;
    public LayerMask wallLayer;

    private Rigidbody rb;
    private PlayerMovement pm;
    private bool isWallRunning;
    private RaycastHit wallHit;
    private float wallRunPauseTimer;
    private bool wallOnRight;
    private bool timeoutExpired;

    private InputAction moveAction;
    private InputAction jumpAction;

    // Initializes references and sets up input bindings
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        var playerActionMap = pm.inputActionAsset.FindActionMap("Player");
        if (playerActionMap != null)
        {
            jumpAction = playerActionMap.FindAction("Jump");
            moveAction = playerActionMap.FindAction("Move");
            jumpAction.Enable();
            moveAction.Enable();
            Debug.Log("Player input actions enabled successfully.");
        }
        else
        {
            Debug.LogError("Player Action Map not found!");
        }
    }

    // Continuously checks for walls and handles wallrunning
    void Update()
    {
        WallCheck();
        WallRunningHandler();
    }

    // Detects nearby walls and determines whether wallrunning should start or stop
    private void WallCheck()
    {
        bool wallDetected = false;

        if (Physics.Raycast(transform.position, pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            wallDetected = true;
            wallOnRight = true;
            Debug.Log("Wall detected on the right side.");
        }
        else if (Physics.Raycast(transform.position, -pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            wallDetected = true;
            wallOnRight = false;
            Debug.Log("Wall detected on the left side.");
        }

        // Reset timeout if the player leaves the wall
        if (timeoutExpired && !wallDetected)
        {
            timeoutExpired = false;
            Debug.Log("Timeout expired and reset as no wall is detected.");
        }

        // Start or stop wallrunning based on wall detection and timeout
        if (wallDetected && !isWallRunning && !timeoutExpired)
        {
            Debug.Log("Conditions met. Starting wallrunning.");
            StartWallRun();
        }
        else if (!wallDetected && isWallRunning)
        {
            Debug.Log("Wall not detected. Stopping wallrunning.");
            StopWallRun();
        }
    }

    // Handles the movement and gravity logic while wallrunning
    private void WallRunningHandler()
    {
        if (isWallRunning)
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            // Determine wallrun direction based on wall side and forward input
            Vector3 wallForward = Vector3.Cross(wallHit.normal, Vector3.up).normalized;
            Vector3 forwardDirection = wallOnRight ? -wallForward : wallForward;

            // Apply forward velocity if the player holds forward
            if (moveInput.y > 0)
            {
                rb.velocity = forwardDirection * wallRunSpeed;
                wallRunPauseTimer = wallRunTimeout;
            }
            else
            {
                // Apply simulated gravity when no forward input
                rb.useGravity = false;
                Vector3 wallGravity = -wallHit.normal * wallGravityForce;
                rb.AddForce(wallGravity, ForceMode.Acceleration);

                // Stop wallrunning if no forward input for too long
                wallRunPauseTimer -= Time.deltaTime;
                if (wallRunPauseTimer <= 0)
                {
                    timeoutExpired = true;
                    Debug.Log("Wallrun timeout expired due to no forward input.");
                    StopWallRun();
                    return;
                }
            }

            // Apply vertical movement based on horizontal input (left/right)
            if (moveInput.x != 0)
            {
                Vector3 verticalMovement = wallOnRight
                    ? (moveInput.x > 0 ? Vector3.up : Vector3.down)
                    : (moveInput.x < 0 ? Vector3.up : Vector3.down);

                rb.velocity += verticalMovement * wallRunSpeed * 0.3f;
            
            }

            // Perform a wall jump if the jump button is pressed
            if (jumpAction.triggered)
            {
                Debug.Log("Wall jump triggered.");
                WallJump();
            }
        }
    }

    // Starts the wallrunning state
    private void StartWallRun()
    {
        if (!isWallRunning && pm.state == PlayerMovement.MovementState.air && pm.grounded == false)
        {
            isWallRunning = true;
            pm.state = PlayerMovement.MovementState.wallrunning;
            wallRunPauseTimer = wallRunTimeout;

            // Reset vertical velocity to avoid abrupt drops
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            Debug.Log("Wallrunning started.");
        }
    }

    // Stops the wallrunning state
    private void StopWallRun()
    {
        if (isWallRunning)
        {
            isWallRunning = false;
            pm.state = PlayerMovement.MovementState.air;
            rb.useGravity = true;
            Debug.Log("Wallrunning stopped.");
        }
    }

    // Handles the wall jump logic
    private void WallJump()
    {
        rb.useGravity = true;
        Vector3 jumpDirection = (wallHit.normal + Vector3.up).normalized;
        rb.velocity = Vector3.zero; // Reset velocity to ensure smooth jump
        rb.AddForce(jumpDirection * wallRunJumpForce, ForceMode.Impulse);

        Debug.Log("Walljump performed.");
        StopWallRun();
    }

    // Returns whether the player is currently wallrunning
    public bool IsWallRunning()
    {
        Debug.Log($"Checking if wallrunning: {isWallRunning}");
        return isWallRunning;
    }
}
