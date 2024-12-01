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

    void Update()
    {
        WallCheck();
        WallRunningHandler();
    }

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

        // If the wall was detected but the timeout expired, wait for the player to leave the wall before re-enabling wallrun
        if (timeoutExpired && !wallDetected)
        {
            timeoutExpired = false; 
        }

        // Start wallrun only if the wall is detected and the timeout flag is not active
        if (wallDetected && !isWallRunning && !timeoutExpired)
        {
            StartWallRun();
        }
        else if (!wallDetected && isWallRunning)
        {
            StopWallRun();
        }
    }

    private void WallRunningHandler()
    {
        if (isWallRunning)
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            // Set wallrun direction based on wall side and forward input
            Vector3 wallForward = Vector3.Cross(wallHit.normal, Vector3.up).normalized;
            Vector3 forwardDirection = wallOnRight ? -wallForward : wallForward;

            if (moveInput.y > 0)
            {
                rb.velocity = forwardDirection * wallRunSpeed;
                wallRunPauseTimer = wallRunTimeout;
            }
            else
            {
                rb.useGravity = false;
                Vector3 wallGravity = -wallHit.normal * wallGravityForce;
                rb.AddForce(wallGravity, ForceMode.Acceleration);

                // Start countdown to stop wallrun if no forward input
                wallRunPauseTimer -= Time.deltaTime;
                if (wallRunPauseTimer <= 0)
                {
                    timeoutExpired = true;
                    StopWallRun();
                    return;
                }
            }

            // Apply vertical movement if horizontal input is given (left/right)
            if (moveInput.x != 0)
            {
                Vector3 verticalMovement = wallOnRight
                    ? (moveInput.x > 0 ? Vector3.up : Vector3.down)
                    : (moveInput.x < 0 ? Vector3.up : Vector3.down);

                rb.velocity += verticalMovement * wallRunSpeed * 0.3f;
            }

            if (jumpAction.triggered)
            {
                WallJump();
            }
        }
    }

    private void StartWallRun()
    {
        if (!isWallRunning && pm.state == PlayerMovement.MovementState.air && pm.grounded == false)
        {
            isWallRunning = true;
            pm.state = PlayerMovement.MovementState.wallrunning;
            wallRunPauseTimer = wallRunTimeout;

            // Clear vertical velocity to prevent abrupt drops
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        }
    }

    private void StopWallRun()
    {
        if (isWallRunning)
        {
            isWallRunning = false;
            pm.state = PlayerMovement.MovementState.air;
            rb.useGravity = true;

        }
    }

    private void WallJump()
    {
        rb.useGravity = true;
        Vector3 jumpDirection = (wallHit.normal + Vector3.up).normalized;
        rb.velocity = Vector3.zero;  // Reset velocity to ensure smooth jump
        rb.AddForce(jumpDirection * wallRunJumpForce, ForceMode.Impulse);

        StopWallRun();
    }

    public bool IsWallRunning()
    {
        return isWallRunning;
    }
}