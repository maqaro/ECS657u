using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float dashSpeed;
    public float dashSpeedChangeFactor;

    public float acceleration = 10f;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.5f;
    private bool readyToJump = true;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Gravity Settings")]
    [SerializeField] private float customGravity = 40f;
    [SerializeField] private float airDrag = 1f;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool isGrounded;

    [Header("Grapple")]
    public bool freeze;
    public bool activeGrapple;
    public Vector3 velocityToSet;
    private bool enableMovementOnTouch;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    private Rigidbody rb;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private Vector3 slopeNormal;

    public MovementState state;

    public enum MovementState
    {
        freeze,
        walking,
        sprinting,
        dashing,
        crouching,
        air
    }

    public bool dashing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        startYScale = transform.localScale.y;
    }

    void FixedUpdate()
    {
        MovePlayer();
        
        // Only apply custom gravity if not on a slope
        if (!OnSlope())
        {
            ApplyCustomGravity();
        }
    }

    private void ApplyCustomGravity()
    {
        rb.AddForce(Vector3.down * customGravity, ForceMode.Acceleration);
    }

    void Update()
    {
        isGrounded = (Mathf.Abs(rb.velocity.y) < 0.001f) ? true : false;

        MyInput();
        speedControl();
        StateHandler();

        rb.drag = (isGrounded && !activeGrapple && state != MovementState.dashing) ? groundDrag : airDrag;

    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(resetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;

    private void StateHandler()
    {

        if (dashing){
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        else if (freeze)
        {
            state = MovementState.freeze;
            desiredMoveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else
        {
            state = MovementState.air;
            if(desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(lastState == MovementState.dashing)
            keepMomentum = true;

        if(desiredMoveSpeedHasChanged){
            if(keepMomentum){
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            } 
            else {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }

        }


        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private float speedChangeFactor;

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }


    private void MovePlayer()
    {
        if (activeGrapple)
        {
            return;
        }

        Vector3 targetDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;
        Vector3 targetVelocity = targetDirection * moveSpeed;

        if (OnSlope())
        {
            Vector3 slopeMoveDirection = GetSlopeMoveDirection() * moveSpeed;
            rb.velocity = Vector3.Lerp(rb.velocity, slopeMoveDirection, acceleration * Time.deltaTime);
        }
        else if (isGrounded)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z), acceleration * Time.deltaTime);
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(targetVelocity.x * airMultiplier, rb.velocity.y, targetVelocity.z * airMultiplier), acceleration * Time.deltaTime);
        }
    }

    private void speedControl()
    {
        if (activeGrapple)
        {
            return;
        }

        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        float currentSpeed = flatVel.magnitude;

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void resetJump()
    {
        readyToJump = true;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.3f, whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            slopeNormal = hit.normal;
            return angle < maxSlopeAngle && angle > 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(orientation.forward * verticalInput + orientation.right * horizontalInput, slopeNormal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private void SetVelocity()
    {
        enableMovementOnTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
        enableMovementOnTouch = false;
    }

    private void OnCollisionEnter(Collision other) {
        if (enableMovementOnTouch){
            ResetRestrictions();
            enableMovementOnTouch = false;
            GetComponent<Grapple>().StopGrapple();
        }
    }
}