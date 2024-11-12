using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public float wallRunSpeed;
    public float wallRunDuration;
    public float wallRunJumpForce;

    [Header("Detection")]
    public float wallCheckDistance;
    public float maxWallRunAngle;
    public LayerMask wallLayer;
    
    private Rigidbody rb;
    private PlayerMovement pm;
    private bool isWallRunning;
    private float wallRunTimer;
    private RaycastHit wallHit;
    
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        // Get Jump action from PlayerMovement's input action asset
        var playerActionMap = pm.inputActionAsset.FindActionMap("Player");
        if (playerActionMap != null)
        {
            jumpAction = playerActionMap.FindAction("Jump");
            jumpAction.Enable();
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
            Debug.Log("Wall detected RIGHT");
            if (Vector3.Angle(Vector3.up, wallHit.normal) < maxWallRunAngle)
            {
                wallDetected = true;
                StartWallRun();
            }
        }
        else if (Physics.Raycast(transform.position, -pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            Debug.Log("Wall detected LEFT");
            if (Vector3.Angle(Vector3.up, wallHit.normal) < maxWallRunAngle)
            {
                wallDetected = true;
                StartWallRun();
            }
        }

        if (!wallDetected)
        {
            StopWallRun();
        }
    }

    private void WallRunningHandler()
    {
        if (isWallRunning)
        {
            wallRunTimer -= Time.deltaTime;
            if (wallRunTimer <= 0)
            {
                StopWallRun();
            }

            Vector3 wallDirection = Vector3.Cross(wallHit.normal, Vector3.up);
            rb.velocity = wallDirection * wallRunSpeed;
            
            // Use the Jump action from the new Input System for wall jumping
            if (jumpAction.triggered)
            {
                WallJump();
            }
        }
    }

    private void StartWallRun()
    {
        if (!isWallRunning)
        {
            isWallRunning = true;
            wallRunTimer = wallRunDuration;
            pm.state = PlayerMovement.MovementState.wallrunning;
            rb.useGravity = false;
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
        Vector3 jumpDirection = wallHit.normal + Vector3.up;
        rb.velocity = Vector3.zero;
        rb.AddForce(jumpDirection * wallRunJumpForce, ForceMode.Impulse);
        StopWallRun();
    }

    public bool IsWallRunning()
    {
        return isWallRunning;
    }
}
