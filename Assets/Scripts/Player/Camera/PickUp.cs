using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System namespace

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public GameObject sword;
    public Transform holdPos;
    public Camera playerCamera;
    private Animator swordAnimator;
    private float originalSenX = 0f;
    private float originalSenY = 0f;

    public float throwForce = 500f; 
    public float pickUpRange = 50f; 
    private float rotationSensitivity = 1f; 
    private GameObject heldObj;
    private Rigidbody heldObjRb; 
    private bool canDrop = true; 
    private int LayerNumber; 

    PlayerCam PlayerCamScript;

    // New Input System variables
    public InputActionAsset inputActionAsset;
    private InputAction pickUpAction;
    private InputAction throwAction;
    private InputAction rotateAction;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("HoldLayer"); 
        swordAnimator = sword.GetComponent<Animator>();
        PlayerCamScript = playerCamera.GetComponent<PlayerCam>();

        // Find and assign actions from the Input Action Asset
        var gameplayActionMap = inputActionAsset.FindActionMap("Player");

        pickUpAction = gameplayActionMap.FindAction("PickUp");
        throwAction = gameplayActionMap.FindAction("Throw");
        rotateAction = gameplayActionMap.FindAction("Rotate");

        // Subscribe to action events
        pickUpAction.performed += _ => HandlePickUpDrop();
        throwAction.performed += _ => HandleThrow();
        rotateAction.performed += _ => StartRotation();
        rotateAction.canceled += _ => StopRotation();

        // Enable actions
        pickUpAction.Enable();
        throwAction.Enable();
        rotateAction.Enable();
    }


    void Update()
    {
        if (heldObj != null)
        {
            MoveObject();
            RotateObject(); // Will only rotate if rotation key is held
        }
    }

    // Handle the pick up and drop actions
    private void HandlePickUpDrop()
    {
        if (heldObj == null) // Attempt to pick up an object
        {
            RaycastHit hit;
            float sphereRadius = 0.5f;

            if (Physics.SphereCast(playerCamera.transform.position, sphereRadius, playerCamera.transform.forward, out hit, pickUpRange))
            {
                // Check if the object can be picked up
                if (hit.transform.gameObject.tag == "canPickUp")
                {
                    // Pick up the object
                    PickUpObject(hit.transform.gameObject);
                }
            }
        }
        else // Drop the object if currently holding one
        {
            if (canDrop)
            {
                StopClipping(); 
                DropObject();
            }
        }
    }

    // Handle the throw action
    private void HandleThrow()
    {
        // Throw the object if currently holding one
        if (heldObj != null && canDrop)
        {
            StopClipping();
            ThrowObject();
        }
    }

    // Pick up the object
    private void PickUpObject(GameObject pickUpObj)
    {
        // Check if the object has a rigidbody
        if (pickUpObj.GetComponent<Rigidbody>()) 
        {
            // Disable the sword animation and set the sword inactive
            swordAnimator.enabled = false;
            sword.SetActive(false);
            
            // Pick up the object
            heldObj = pickUpObj; 
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber; 

            // Prevent the object from colliding with the player
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    // Drop the object
    private void DropObject()
    {
        // Enable the sword animation and set the sword active
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0; 
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; 
        heldObj = null; 

        sword.SetActive(true);
        swordAnimator.enabled = true;
    }

    private void MoveObject()
    {
        // Move the object to the hold position
        heldObj.transform.position = holdPos.transform.position;
    }

    // Start rotating the object
    private void StartRotation()
    {
        canDrop = false; // Prevent the object from being dropped while rotating

        if (originalSenX == 0 && originalSenY == 0) // Store the original camera sensitivity
        {
            originalSenX = PlayerCamScript.senX;
            originalSenY = PlayerCamScript.senY;
        }

        PlayerCamScript.senX = 0f;
        PlayerCamScript.senY = 0f;
    }

    // Rotate the object
    private void RotateObject()
    {
        // Rotate the object based on mouse input
        if (rotateAction.IsPressed())
        {
            float XaxisRotation = Mouse.current.delta.x.ReadValue() * rotationSensitivity;
            float YaxisRotation = Mouse.current.delta.y.ReadValue() * rotationSensitivity;

            heldObj.transform.Rotate(Vector3.down, XaxisRotation);   
            heldObj.transform.Rotate(Vector3.right, YaxisRotation); 
        }
    }

    // Stop rotating the object
    private void StopRotation()
    {
        // Reset the camera sensitivity to its original value
        if (originalSenX != 0 && originalSenY != 0)
        {
            PlayerCamScript.senX = originalSenX;
            PlayerCamScript.senY = originalSenY;

            originalSenX = 0;
            originalSenY = 0;
        }

        // Allow the object to be dropped
        canDrop = true;
    }

    // Throw the object
    private void ThrowObject()
    {
        // Throw the object 
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;

        sword.SetActive(true);
        swordAnimator.enabled = true;
    }

    // Prevent the object from clipping through walls
    private void StopClipping()
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
       
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); 
        }
    }
}
