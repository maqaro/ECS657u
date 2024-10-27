using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float swingSpeed;
    public float climbSpeed;
    public float maxYSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public float fallMultiplier = 2.5f;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Dashing")]
    public bool dashing;
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    private float speedChangeFactor; // reintroduced

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Camera")]
    public Transform orientation;

    private Vector2 moveInput;
    private bool jumpInput;
    private bool crouchInput;
    private bool sprintInput;

    Vector3 moveDirection; // reintroduced

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        grappling,
        climbing,
        dashing,
        walking,
        sprinting,
        crouching,
        air
    }

    public bool freeze;
    public bool climbing;

    [Header("Grapple Settings")]
    public bool activeGrapple;
    public bool swinging;
    private bool enableMovementOnNextTouch;
    private Vector3 velocityToSet;

    // Input Action Asset Reference
    public InputActionAsset inputActionAsset;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;

void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;

        // Set up the input actions
        var playerActionMap = inputActionAsset.FindActionMap("Player");

        if (playerActionMap != null)
        {
            // Find the actions
            moveAction = playerActionMap.FindAction("Move");
            jumpAction = playerActionMap.FindAction("Jump");
            crouchAction = playerActionMap.FindAction("Crouch");
            sprintAction = playerActionMap.FindAction("Sprint");

            moveAction?.Enable();
            jumpAction?.Enable();
            crouchAction?.Enable();
            sprintAction?.Enable();

            // Bind jump and crouch input actions
            jumpAction.performed += ctx => OnJump();
            crouchAction.performed += ctx => OnCrouchStart();
            crouchAction.canceled += ctx => OnCrouchEnd();

            Debug.Log("Player Action Map found!");
        }
        else
        {
            Debug.Log("Player Action Map not found!");
        }
    }

    void FixedUpdate()
    {
        // Handle movement
        MovePlayer();

        // Handle gravity
        if (state == MovementState.air || (state == MovementState.crouching && !grounded))
        {
            // Apply gravity multiplier when falling
            rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }
    }

    void Update()
    {
        // Check if the player is grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        speedControl();
        StateHandler();

        // Handle the drag
        if (grounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void MyInput()
    {
        // Read input values
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        moveInput = new Vector2(inputVector.x, inputVector.y);

        sprintInput = sprintAction.ReadValue<float>() > 0;
    }

    private void OnJump()
    {
        // Check if the player is ready to jump and grounded
        if (readyToJump && grounded)
        {
            // Set the jump flag to false and jump
            readyToJump = false;
            Jump();
            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void OnCrouchStart()
    {
        // Set the crouch flag to true and crouch
        crouchInput = true;
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    private void OnCrouchEnd()
    {
        // Set the crouch flag to false and uncrouch
        crouchInput = false;
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    public void StateHandler()
    {
        // Handle the player's state
        if (freeze)
        {
            // Freeze the player
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else if (activeGrapple)
        {
            // Set the player's state to grappling
            state = MovementState.grappling;
            moveSpeed = sprintSpeed;
        }
        else if (dashing)
        {
            // Set the player's state to dashing
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        else if (climbing)
        {
            // Set the player's state to climbing
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }
        else if (crouchInput)
        {
            // Set the player's state to crouching
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && sprintInput)
        {
            // Set the player's state to sprinting
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            // Set the player's state to walking
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            // Set the player's state to air
            state = MovementState.air;
        }

        // Handle the player's movement speed
        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        // Smoothly lerp the player's movement speed
        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                // Stop all coroutines and start the SmoothlyLerpMoveSpeed coroutine
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                // Stop all coroutines and set the player's movement speed to the desired movement speed
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        // Update the last desired movement speed and state
        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private IEnumerator SmoothlyLerpMoveSpeed() // reintroduced coroutine
    {
        // Smoothly lerp the player's movement speed
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;
        float boostFactor = speedChangeFactor;

        // Lerp the player's movement speed
        while (time < difference)
        {
            // Lerp the player's movement speed
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * boostFactor;
            yield return null;
        }

        // Set the player's movement speed to the desired movement speed
        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing)
            return;

        // Handle the player's movement
        if (activeGrapple || MovementState.dashing == state)
        {
            return;
        }

        // Move the player
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x; // reintroduced

        // Handle the player's movement
        if (OnSlope() && !exitingSlope)
        {
            // Move the player on the slope
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded)
        {
            // Move the player on the ground
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            // Move the player in the air
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        else
        {
            // Stop the player
            rb.useGravity = !OnSlope();
        }
    }

    private void speedControl()
    {
        if (activeGrapple)
        {
            // Set the player's movement speed to the swing speed
            return;
        }

        if (OnSlope() && !exitingSlope)
        {
            // Set the player's movement speed to the climb speed
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            // Set the player's movement speed to the walk speed
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        // Limit the player's y velocity
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    private void Jump()
    {
        // Jump
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void resetJump()
    {
        // Reset the jump flag
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        // Check if the player is on a slope
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        // Get the slope move direction
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    // Jump to position
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        // Calculate the jump velocity
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) +
            Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    // Grapple
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        // Jump to the target position
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    // Set the velocity
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    // Reset the restrictions 
    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    // Stop the grapple
    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<Grapple>().StopGrapple();
        }
    }

}