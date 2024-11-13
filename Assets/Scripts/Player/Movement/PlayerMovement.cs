using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    private float originalColliderHeight;

    [Header("Dashing")]
    public bool dashing;
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    private float speedChangeFactor;

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
    public Transform cameraTransform;

    private Vector2 moveInput;
    private bool jumpInput;
    private bool crouchInput;
    private bool sprintInput;

    Vector3 moveDirection;

    Rigidbody rb;
    private CapsuleCollider playerCollider;

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
        air,
        wallrunning
    }

    public bool freeze;
    public bool climbing;

    [Header("Grapple Settings")]
    public bool activeGrapple;
    public bool swinging;
    private bool enableMovementOnNextTouch;
    private Vector3 velocityToSet;

    [Header("Wall Running")]
    public WallRunning wallRunningScript;

    public InputActionAsset inputActionAsset;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;

    [Header("Animations")]
    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        playerCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = playerCollider != null ? playerCollider.height : 0;
        startYScale = cameraTransform != null ? cameraTransform.localPosition.y : 0;

        var playerActionMap = inputActionAsset.FindActionMap("Player");

        if (playerActionMap != null)
        {
            moveAction = playerActionMap.FindAction("Move");
            jumpAction = playerActionMap.FindAction("Jump");
            crouchAction = playerActionMap.FindAction("Crouch");
            sprintAction = playerActionMap.FindAction("Sprint");

            moveAction?.Enable();
            jumpAction?.Enable();
            crouchAction?.Enable();
            sprintAction?.Enable();

            jumpAction.performed += ctx => OnJump();
            crouchAction.performed += ctx => OnCrouchStart();
            crouchAction.canceled += ctx => OnCrouchEnd();
        }
        else
        {
            Debug.Log("Player Action Map not found!");
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (jumpAction != null) jumpAction.performed -= ctx => OnJump();
        if (crouchAction != null)
        {
            crouchAction.performed -= ctx => OnCrouchStart();
            crouchAction.canceled -= ctx => OnCrouchEnd();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider>();

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
        }

        originalColliderHeight = playerCollider != null ? playerCollider.height : 0;
        startYScale = cameraTransform != null ? cameraTransform.localPosition.y : 0;
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (state == MovementState.air || (state == MovementState.crouching && !grounded))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        speedControl();
        StateHandler();
        handleAnimations();

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
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        moveInput = new Vector2(inputVector.x, inputVector.y);

        sprintInput = sprintAction.ReadValue<float>() > 0;
    }

    private void OnJump()
    {
        if (cameraTransform == null || playerCollider == null)
        {
            return;
        }
        if (readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void OnCrouchStart()
    {
        if (cameraTransform == null || playerCollider == null)
        {
            return;
        }

        crouchInput = true;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, crouchYScale, cameraTransform.localPosition.z);
        playerCollider.height = originalColliderHeight * 0.5f;
        transform.position = new Vector3(transform.position.x, transform.position.y - (originalColliderHeight * 0.25f), transform.position.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    private void OnCrouchEnd()
    {
        if (cameraTransform == null || playerCollider == null)
        {
            return;
        }

        crouchInput = false;
        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, startYScale, cameraTransform.localPosition.z);
        playerCollider.height = originalColliderHeight;
        transform.position = new Vector3(transform.position.x, transform.position.y + (originalColliderHeight * 0.25f), transform.position.z);
    }

    public void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = sprintSpeed;
        }
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        else if (wallRunningScript != null && wallRunningScript.IsWallRunning())
        {
            state = MovementState.wallrunning;
            moveSpeed = wallRunningScript.wallRunSpeed;
        }
        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }
        else if (crouchInput)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && sprintInput)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

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
        if (activeGrapple || MovementState.dashing == state)
        {
            return;
        }

        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        else
        {
            rb.useGravity = !OnSlope();
        }
    }

    private void speedControl()
    {
        if (activeGrapple)
        {
            return;
        }

        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void resetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) +
            Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

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
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<Grapple>().StopGrapple();
        }
    }

    private void handleAnimations()
    {
        if (moveInput == Vector2.zero || state == MovementState.crouching)
        {
            anim.SetFloat("Blend", 0f, 0.2f, Time.deltaTime);
        }
        else if (state == MovementState.sprinting)
        {
            anim.SetFloat("Blend", 1f, 0.2f, Time.deltaTime);
        }
        else if (state == MovementState.walking)
        {
            anim.SetFloat("Blend", 0.5f, 0.2f, Time.deltaTime);
        }
    }
}
