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
}

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
        Debug.Log("Player input actions enabled.");
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

    if (timeoutExpired)
    {
        Debug.Log("Wallrunning temporarily disabled due to timeout.");
    }
}

// Detects nearby walls and determines whether wallrunning should start or stop
private void WallCheck()
{
    bool wallDetected = false;

    if (Physics.Raycast(transform.position, pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
    {
        wallDetected = true;
        wallOnRight = true;
        Debug.Log("Wall detected on the right.");
    }
    else if (Physics.Raycast(transform.position, -pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
    {
        wallDetected = true;
        wallOnRight = false;
        Debug.Log("Wall detected on the left.");
    }

    if (timeoutExpired && !wallDetected)
    {
        timeoutExpired = false;
        Debug.Log("Wall timeout reset.");
    }

    if (wallDetected && !isWallRunning && !timeoutExpired)
    {
        Debug.Log("Conditions met to start wallrunning.");
        StartWallRun();
    }
    else if (!wallDetected && isWallRunning)
    {
        Debug.Log("Wallrunning stopped because no wall was detected.");
        StopWallRun();
    }
}

// Handles the movement and gravity logic while wallrunning
private void WallRunningHandler()
{
    if (isWallRunning)
    {
        Debug.Log("Wallrunning in progress...");
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        Vector3 wallForward = Vector3.Cross(wallHit.normal, Vector3.up).normalized;
        Vector3 forwardDirection = wallOnRight ? -wallForward : wallForward;

        if (moveInput.y > 0)
        {
            rb.velocity = forwardDirection * wallRunSpeed;
            wallRunPauseTimer = wallRunTimeout;
            Debug.Log("Applying forward velocity while wallrunning.");
        }
        else
        {
            rb.useGravity = false;
            Vector3 wallGravity = -wallHit.normal * wallGravityForce;
            rb.AddForce(wallGravity, ForceMode.Acceleration);
            Debug.Log("Simulating gravity while wallrunning.");

            wallRunPauseTimer -= Time.deltaTime;
            if (wallRunPauseTimer <= 0)
            {
                timeoutExpired = true;
                Debug.Log("Wallrun timeout expired.");
                StopWallRun();
                return;
            }
        }

        if (moveInput.x != 0)
        {
            Vector3 verticalMovement = wallOnRight
                ? (moveInput.x > 0 ? Vector3.up : Vector3.down)
                : (moveInput.x < 0 ? Vector3.up : Vector3.down);

            rb.velocity += verticalMovement * wallRunSpeed * 0.3f;
            Debug.Log("Vertical movement applied during wallrun.");
        }

        if (jumpAction.triggered)
        {
            Debug.Log("Walljump triggered.");
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

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Debug.Log("Wallrunning state started.");
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
        Debug.Log("Wallrunning state stopped.");
    }
}

// Handles the wall jump logic
private void WallJump()
{
    rb.useGravity = true;
    Vector3 jumpDirection = (wallHit.normal + Vector3.up).normalized;
    rb.velocity = Vector3.zero;
    rb.AddForce(jumpDirection * wallRunJumpForce, ForceMode.Impulse);

    Debug.Log("Walljump performed.");
    StopWallRun();
}

// Returns whether the player is currently wallrunning
public bool IsWallRunning()
{
    Debug.Log($"IsWallRunning called: {isWallRunning}");
    return isWallRunning;
}
