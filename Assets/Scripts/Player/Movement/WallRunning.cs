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
    public float wallRunTimeout = 1.5f;

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

        // Check for walls on the left or right of the player
        if (Physics.Raycast(transform.position, pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            wallDetected = true;
            wallOnRight = true;
            StartWallRun();
        }
        else if (Physics.Raycast(transform.position, -pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            wallDetected = true;
            wallOnRight = false;
            StartWallRun();
        }

        // Stop wallrunning if no wall is detected
        if (!wallDetected && isWallRunning)
        {
            StopWallRun();
        }
    }

    private void WallRunningHandler()
    {
        if (isWallRunning)
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            // Wallrun in the direction the player is facing, without forced backward/forward movement
            Vector3 wallForward = Vector3.Cross(wallHit.normal, Vector3.up).normalized;
            Vector3 forwardDirection = Vector3.Dot(transform.forward, wallForward) > 0 ? wallForward : -wallForward;

            // Apply forward movement along the wall if moving forward
            if (moveInput.y > 0)
            {
                rb.velocity = forwardDirection * wallRunSpeed;
                wallRunPauseTimer = wallRunTimeout;  // Reset the pause timer
            }
            else
            {
                // Start pause timer if not pressing forward
                wallRunPauseTimer -= Time.deltaTime;
                if (wallRunPauseTimer <= 0)
                {
                    StopWallRun();
                    return;
                }
            }

            // Handle upward and downward movement only if horizontal input is present
            if (moveInput.x != 0)
            {
                Vector3 verticalMovement = wallOnRight
                    ? (moveInput.x > 0 ? Vector3.up : Vector3.down)
                    : (moveInput.x < 0 ? Vector3.up : Vector3.down);

                rb.velocity += verticalMovement * wallRunSpeed * 0.3f;  // Adjusted speed for smoother control
            }

            // Apply custom gravity perpendicular to the wall
            rb.useGravity = false;  // Disable normal gravity
            Vector3 wallGravity = -wallHit.normal * wallGravityForce;
            rb.AddForce(wallGravity, ForceMode.Acceleration);

            // Stop wallrunning if the jump action is triggered
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
            rb.useGravity = true;  // Re-enable normal gravity

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
