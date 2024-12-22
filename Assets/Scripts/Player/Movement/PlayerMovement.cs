using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

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

    public Vector2 moveInput;
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
        wallrunning,
        interacting
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
    public InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;
    private InputAction interactAction;



    void Start()
    {
        // gets  rigidbody components
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        // gets player collider component
        playerCollider = GetComponent<CapsuleCollider>();
        originalColliderHeight = playerCollider != null ? playerCollider.height : 0;
        startYScale = cameraTransform != null ? cameraTransform.localPosition.y : 0;

        var playerActionMap = inputActionAsset.FindActionMap("Player");

        // checks if PlayerAction map is not empty
        if (playerActionMap != null)
        {
            // set up the actions
            moveAction = playerActionMap.FindAction("Move");
            jumpAction = playerActionMap.FindAction("Jump");
            crouchAction = playerActionMap.FindAction("Crouch");
            sprintAction = playerActionMap.FindAction("Sprint");
            interactAction = playerActionMap.FindAction("Interact");

            moveAction?.Enable();
            jumpAction?.Enable();
            crouchAction?.Enable();
            sprintAction?.Enable();
            interactAction?.Enable();

            // Bind jump and crouch input actions
            jumpAction.performed += ctx => OnJump();
            crouchAction.performed += ctx => OnCrouchStart();
            interactAction.performed += ctx => OnInteract();
            crouchAction.canceled += ctx => OnCrouchEnd();
        }
        else
        {
            // print error if empty
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
        if (interactAction != null) interactAction.performed -= ctx => OnInteract();
        if (crouchAction != null)
        {
            crouchAction.performed -= ctx => OnCrouchStart();
            crouchAction.canceled -= ctx => OnCrouchEnd();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-assign playerCollider and cameraTransform in case of scene switch
        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider>();

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
        }
        
        // Set initial values based on newly assigned references
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
        // ray cast to check if the player is on the ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // handle method calls
        MyInput();
        speedControl();
        StateHandler();

        // handle gravity and drag
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
        if (cameraTransform == null || playerCollider == null)
        {
            return;
        }

        // Check if the player is ready to jump and grounded
        if (readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    // Interacting
    private void OnInteract()
    {
        if (dialogueUI.IsDialogueActive) return;

        if (Interactable != null)
        {
            state = MovementState.interacting;
            rb.velocity = Vector3.zero;
            Interactable.Interact(player: this);
        }
    }

    public void EndInteraction()
    {
        if (state == MovementState.interacting)
        {
            state = MovementState.walking; 
            MovePlayer();
        }
    }


    private void SmoothSpeedChange(float targetSpeed, float duration){
        DOTween.To(() => moveSpeed, x => moveSpeed = x, targetSpeed, duration).SetEase(Ease.InOutQuad);
    }

    // crouching
    private void OnCrouchStart()
    {
        if (cameraTransform == null || playerCollider == null)
        {
            return;
        }

        crouchInput = true;
        cameraTransform.DOLocalMoveY(crouchYScale, 0.3f); // Smoothly lower the camera
        playerCollider.height = originalColliderHeight * 0.5f;
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    // uncrouching
    private void OnCrouchEnd()
    {
        if (cameraTransform == null || playerCollider == null)
        {
            return;
        }

        crouchInput = false;
        cameraTransform.DOLocalMoveY(startYScale, 0.3f); // Smoothly raise the camera
        playerCollider.height = originalColliderHeight;
    }

    // handling the movement state
    public void StateHandler()
    {
        // Freeze state stops all movement
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
            return;
        }

        // Handle grappling
        if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = sprintSpeed; // Maintain sprint speed during grapple
            return;
        }

        if (state == MovementState.interacting)
        {
            moveInput = Vector2.zero;
            sprintInput = false;
            jumpInput = false;
            return;
        }

        // Handle dashing
        if (dashing)
        {
            state = MovementState.dashing;
            SmoothSpeedChange(dashSpeed, 0.1f); // Smooth transition to dash speed
            return;
        }

        // Handle wall running
        if (wallRunningScript != null && wallRunningScript.IsWallRunning())
        {
            state = MovementState.wallrunning;
            moveSpeed = wallRunningScript.wallRunSpeed;
            return;
        }

        // Handle climbing
        if (climbing)
        {
            state = MovementState.climbing;
            SmoothSpeedChange(climbSpeed, 0.3f); // Smooth transition to climb speed
            return;
        }

        // Handle crouching
        if (crouchInput)
        {
            state = MovementState.crouching;
            SmoothSpeedChange(crouchSpeed, 0.3f); // Smooth transition to crouch speed
            return;
        }

        // Handle sprinting
        if (grounded && sprintInput)
        {
            state = MovementState.sprinting;

            // Set speed immediately to avoid slow acceleration
            if (lastState != MovementState.sprinting)
            {
                moveSpeed = sprintSpeed;
            }
            else
            {
                SmoothSpeedChange(sprintSpeed, 0.2f); // Smooth transition if already sprinting
            }
            return;
        }

        // Handle walking
        if (grounded)
        {
            state = MovementState.walking;

            // Set speed immediately to avoid slow acceleration
            if (lastState != MovementState.walking)
            {
                moveSpeed = walkSpeed;
            }
            else
            {
                SmoothSpeedChange(walkSpeed, 0.2f); // Smooth transition if already walking
            }
            return;
        }


        // Handle air movement
        state = MovementState.air;

        // Maintain walk speed in air without smoothing
        if (lastState != MovementState.air)
        {
            moveSpeed = walkSpeed;
        }

        // Update the last desired move speed and state
        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }



    //private IEnumerator SmoothlyLerpMoveSpeed()
    //{
        // smoothly lerp the players speed
        //float time = 0;
       // float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
       // float startValue = moveSpeed;
      //  float boostFactor = speedChangeFactor;

      //  while (time < difference)
      //  {
      //      moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
           // time += Time.deltaTime * boostFactor;
          //  yield return null;
       // }

      //  moveSpeed = desiredMoveSpeed;
      //  speedChangeFactor = 1f;
     //   keepMomentum = false;
  //  }

    //handle players movement
    private void MovePlayer()
    {
        // if player is grappling or dashing, return to prevent movement
        if (state == MovementState.interacting || activeGrapple || state == MovementState.dashing)
        {
            return; // Skip movement logic
        }

        // move the player
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        // handle slope movement
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        // handle movement while grounded
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        // handle movement in the air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        // else stop the player
        else
        {
            rb.useGravity = !OnSlope();
        }
    }

    private void speedControl()
    {
        // stop the players regular movement speed
        if (activeGrapple)
        {
            return;
        }


        // climbing speed
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            // walk speed
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        // limit player's y velocity
        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    // handle jumping logic
    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // method to reset jumpm called upon landing
    private void resetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    // check if the player is on a slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    // movement direction on a slope
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    // calculate the arc velocity when grappling,
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

    // preform grapple
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    // set players velocity
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }

    // reset grapple restrictions
    public void ResetRestrictions()
    {
        activeGrapple = false;
    }

    // stops the grapple if player collides with a wall during the flight
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