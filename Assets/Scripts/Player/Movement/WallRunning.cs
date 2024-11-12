using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        WallCheck();
        WallRunningHandler();
    }

    private void WallCheck()
    {
        if (Physics.Raycast(transform.position, pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            Debug.Log("Wall detected on the right.");
            if (Vector3.Angle(Vector3.up, wallHit.normal) < maxWallRunAngle)
            {
                StartWallRun();
            }
        }
        else if (Physics.Raycast(transform.position, -pm.orientation.right, out wallHit, wallCheckDistance, wallLayer))
        {
            Debug.Log("Wall detected on the left.");
            if (Vector3.Angle(Vector3.up, wallHit.normal) < maxWallRunAngle)
            {
                StartWallRun();
            }
        }
        else
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
            
            if (Input.GetButtonDown("Jump"))
            {
                WallJump();
            }
        }
    }

    private void StartWallRun()
    {
        if (!isWallRunning)
        {
            Debug.Log("Starting wall run.");
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
            Debug.Log("Stopping wall run.");
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
