using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerMovement pm;
    public LayerMask whatIsWall;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    [Header("ClimbJumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;

    public int climbJumps;
    private int climbJumpsLeft;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Input Action Asset")]
    public InputActionAsset inputActionAsset; // Reference to the InputActionAsset
    private InputAction climbAction; // Climb input action (replaces Input.GetKey(KeyCode.W))
    private InputAction jumpAction; // Jump input action (replaces Input.GetKeyDown(jumpKey))

    private void Start()
    {
        // Initialize the input actions
        var playerActionMap = inputActionAsset.FindActionMap("Player");
        climbAction = playerActionMap.FindAction("Climb");
        jumpAction = playerActionMap.FindAction("Jump");

        // Enable the input actions
        climbAction.Enable();
        jumpAction.Enable();

        // Optional: Subscribe to jumpAction.performed event to trigger ClimbJump
        jumpAction.performed += ctx => ClimbJump();
    }

    private void Update()
    {
        // Check if the player is climbing and update the state machine
        WallCheck();
        StateMachine();

        if (climbing && !exitingWall)
        {
            //
            ClimbingMovement();
        }
    }

    private void StateMachine()
    {
        // Check if climbing action is being performed and the wall is climbable
        if (wallFront && climbAction.ReadValue<float>() > 0 && wallLookAngle < maxWallLookAngle && !exitingWall)
        {
            if (!climbing && climbTimer > 0)
            {
                StartClimbing();
            }

            // Reset the climb timer if the player is still climbing 
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        }
        else if (exitingWall)
        {
            // Stop climbing if the player is exiting the wall
            if (climbing) StopClimbing();

            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer < 0) exitingWall = false;
        }
        else
        {
            // Stop climbing
            if (climbing)
            {
                StopClimbing();
            }
        }

        // Trigger climb jump if conditions are met
        if (wallFront && jumpAction.triggered && climbJumpsLeft > 0)
        {
            ClimbJump();
        }
    }

    private void WallCheck()
    {
        // Perform a sphere cast to detect walls
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);
        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        // Reset the climb timer if the player is grounded or the wall is new
        if ((wallFront && newWall) || pm.grounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimbing()
    {
        // Start climbing
        climbing = true;
        pm.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    private void ClimbingMovement()
    {
        // Move the player up the wall
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    private void StopClimbing()
    {
        // Stop climbing
        climbing = false;
        pm.climbing = false;
    }

    private void ClimbJump()
    {
        if (exitingWall || climbJumpsLeft <= 0) return; // Ensure the player can still jump

        // Perform a climb jump
        exitingWall = true;
        exitWallTimer = exitWallTime;

        // Calculate the force to apply
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;

        // Apply the force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }
}
