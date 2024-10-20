using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    [Header("Grapple Keybind")]
    public KeyCode grappleKey;
    private bool isGrappling;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
    }

    private void LateUpdate() {
        if (isGrappling){
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void Update(){
        if(Input.GetKeyDown(grappleKey) && !isGrappling){
            StartGrapple();
        }

        if (grapplingCDTimer > 0){
            grapplingCDTimer -= Time.deltaTime;
        }
    }

    private void StartGrapple(){
        if (grapplingCDTimer > 0){
            return;
        }

        isGrappling = true;

        pm.freeze = true;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable)){
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        } else {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple(){
        pm.freeze = false;
        isGrappling = false;
        grapplingCDTimer = grappleCooldown;
        lr.enabled = false;
    }
}
