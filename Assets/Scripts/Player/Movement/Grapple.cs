using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grapple : MonoBehaviour
{
    [Header("Grapple References")]
    private PlayerMovement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grapple Settings")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;
    private Vector3 grapplePoint;

    [Header("Grapple Cooldown")]
    public float grappleCooldown;
    private float grapplingCDTimer;

    [Header("Input Action Asset")]
    public InputActionAsset inputActionAsset; // Reference to the InputActionAsset
    private InputAction grappleAction; // Define the grapple action

    private bool isGrappling;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();

        // Initialize the grapple input action from the InputActionAsset
        var playerActionMap = inputActionAsset.FindActionMap("Player");
        grappleAction = playerActionMap.FindAction("Grapple");

        // Enable the grapple action and bind the StartGrapple method to its performed event
        grappleAction.Enable();
        grappleAction.performed += ctx => StartGrapple();
    }

    private void LateUpdate()
    {
        // Update the line renderer
        if (isGrappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void Update()
    {
        // Handle the cooldown timer
        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }
    }

    void OnDisable()
    {
        if (grappleAction != null) grappleAction.performed -= ctx => StartGrapple();
    }

    private void StartGrapple()
    {
        if (grapplingCDTimer > 0 || isGrappling || pm == null || cam == null || lr == null || gunTip == null)
            return;

        isGrappling = true;
        pm.freeze = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }


    private void ExecuteGrapple()
    {
        // Execute the grapple
        pm.freeze = false;

        // Calculate the highest point on the arc
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        // If the grapple point is below the player, set the highest point to the overshoot value
        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pm.freeze = false;
        isGrappling = false;
        grapplingCDTimer = grappleCooldown;
        lr.enabled = false;
    }
}
