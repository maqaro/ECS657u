using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public float wallRunSpeed = 10f;
    public float wallRunJumpForce = 8f;
    public float wallGravityForce = 20f;

    [Header("Detection")]
    public float wallCheckDistance = 1f;
    public float maxWallRunAngle = 60f;
    public LayerMask wallLayer;

    private Rigidbody rb;
    private PlayerMovement pm;
    private bool isWallRunning;
    private RaycastHit wallHit;

    private InputAction moveAction;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        // Get Jump and Move actions from PlayerMovement's input action asset
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

        // Cast to the right
        if (Physics.Raycast(transform.position, pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            Debug.Log("Wall detected on the RIGHT");
            wallDetected = true;
            StartWallRun();
        }
        // Cast to the left
        else if (Physics.Raycast(transform.position, -pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            Debug.Log("Wall detected on the LEFT");
            wallDetected = true;
            StartWallRun();
        }

        // Stop wallrunning if no wall detected
        if (!wallDetected && isWallRunning)
        {
            StopWallRun();
        }
    }

    private void WallRunningHandler()
    {
        if (isWallRunning)
        {
            Vector3 wallDirection = Vector3.Cross(wallHit.normal, Vector3.up).normalized;

            // Apply forward movement along the wall direction
            rb.velocity = wallDirection * wallRunSpeed;

            // Pull the player towards the wall with a gravity force perpendicular to the wall
            Vector3 wallGravity = -wallHit.normal * wallGravityForce;
            rb.AddForce(wallGravity, ForceMode.Acceleration);

            Debug.Log("Wallrunning with velocity: " + rb.velocity);

            // Stop wallrunning if the player presses the opposite direction
            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            if ((wallHit.normal == pm.orientation.right && moveInput.x < 0) || 
                (wallHit.normal == -pm.orientation.right && moveInput.x > 0))
            {
                StopWallRun();
            }

            // Trigger wall jump if Jump action is triggered
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

            // Apply an initial strong force towards the wall to "stick" the player
            rb.AddForce(-wallHit.normal * wallGravityForce * 2, ForceMode.Impulse);

            // Reset any vertical velocity for smooth wall-running
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            Debug.Log("Started wall run");
        }
    }

    private void StopWallRun()
    {
        if (isWallRunning)
        {
            isWallRunning = false;
            pm.state = PlayerMovement.MovementState.air;

            Debug.Log("Stopped wall run");
        }
    }

    private void WallJump()
    {
        Vector3 jumpDirection = (wallHit.normal + Vector3.up).normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(jumpDirection * wallRunJumpForce, ForceMode.Impulse);

        Debug.Log("Wall jump executed");

        StopWallRun();
    }

    public bool IsWallRunning()
    {
        return isWallRunning;
    }
}
