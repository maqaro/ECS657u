using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

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
        walking,
        sprinting,
        crouching,
        air
    }

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

        rb.drag = isGrounded ? groundDrag : airDrag;
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

    private void StateHandler()
    {
        if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
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
}