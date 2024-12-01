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
        }
        else if (Physics.Raycast(transform.position, -pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            wallDetected = true;
            wallOnRight = false;
        }

        // Reset timeout if the player leaves the wall
        if (timeoutExpired && !wallDetected)
        {
            timeoutExpired = false;
        }

        // Start or stop wallrunning based on wall detection and timeout
        if (wallDetected && !isWallRunning && !timeoutExpired)
        {
            StartWallRun();
        }
        else if (!wallDetected && isWallRunning)
        {
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
        }
    }

    // Handles the wall jump logic
    private void WallJump()
    {
        rb.useGravity = true;
        Vector3 jumpDirection = (wallHit.normal + Vector3.up).normalized;
        rb.velocity = Vector3.zero; // Reset velocity to ensure smooth jump
        rb.AddForce(jumpDirection * wallRunJumpForce, ForceMode.Impulse);

        StopWallRun();
    }

    // Returns whether the player is currently wallrunning
    public bool IsWallRunning()
    {
        return isWallRunning;
    }
}
