using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System namespace

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public Camera playerCamera;
    public bool isHolding;
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
        
        // Ensure playerCamera is assigned and get the PlayerCamScript if available
        if (playerCamera != null)
        {
            PlayerCamScript = playerCamera.GetComponent<PlayerCam>();
        }

        // Find and assign actions from the Input Action Asset
        var gameplayActionMap = inputActionAsset.FindActionMap("Player");

        pickUpAction = gameplayActionMap?.FindAction("Interact");
        throwAction = gameplayActionMap?.FindAction("Throw");
        rotateAction = gameplayActionMap?.FindAction("Rotate");

        // Subscribe to action events if actions are available
        if (pickUpAction != null) pickUpAction.performed += HandlePickUpDrop;
        if (throwAction != null) throwAction.performed += HandleThrow;
        if (rotateAction != null)
        {
            rotateAction.performed += StartRotation;
            rotateAction.canceled += StopRotation;
        }

        // Enable actions
        pickUpAction?.Enable();
        throwAction?.Enable();
        rotateAction?.Enable();
    }

    void Update()
    {
        if (heldObj != null)
        {
            isHolding = true;
            MoveObject();
            RotateObject(); // Will only rotate if rotation key is held
        }
        else
        {
            isHolding = false;
        }
    }

    private void HandlePickUpDrop(InputAction.CallbackContext context)
    {
        if (heldObj == null) // Attempt to pick up an object
        {
            RaycastHit hit;
            float sphereRadius = 0.5f;

            // Check if playerCamera is available before performing SphereCast
            if (playerCamera != null && Physics.SphereCast(playerCamera.transform.position, sphereRadius, playerCamera.transform.forward, out hit, pickUpRange))
            {
                if (hit.transform.gameObject.CompareTag("canPickUp"))
                {
                    PickUpObject(hit.transform.gameObject);
                }
            }
        }
        else if (canDrop) // Drop the object if currently holding one
        {
            StopClipping(); 
            DropObject();
        }
    }

    private void HandleThrow(InputAction.CallbackContext context)
    {
        if (heldObj != null && canDrop)
        {
            StopClipping();
            ThrowObject();
        }
    }

    private void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) 
        {


            heldObj = pickUpObj; 
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber;

            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    private void DropObject()
    {
        if (heldObj != null && heldObjRb != null)
        {
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObj = null;


        }
    }

    private void MoveObject()
    {
        if (heldObj != null)
        {
            heldObj.transform.position = holdPos.position;
        }
    }

    private void StartRotation(InputAction.CallbackContext context)
    {
        canDrop = false;

        if (PlayerCamScript != null && originalSenX == 0 && originalSenY == 0) 
        {
            originalSenX = PlayerCamScript.senX;
            originalSenY = PlayerCamScript.senY;
        }

        if (PlayerCamScript != null)
        {
            PlayerCamScript.senX = 0f;
            PlayerCamScript.senY = 0f;
        }
    }

    private void RotateObject()
    {
        if (rotateAction != null && rotateAction.IsPressed() && heldObj != null)
        {
            float XaxisRotation = Mouse.current.delta.x.ReadValue() * rotationSensitivity;
            float YaxisRotation = Mouse.current.delta.y.ReadValue() * rotationSensitivity;

            heldObj.transform.Rotate(Vector3.down, XaxisRotation);   
            heldObj.transform.Rotate(Vector3.right, YaxisRotation); 
        }
    }

    private void StopRotation(InputAction.CallbackContext context)
    {
        if (PlayerCamScript != null && originalSenX != 0 && originalSenY != 0)
        {
            PlayerCamScript.senX = originalSenX;
            PlayerCamScript.senY = originalSenY;

            originalSenX = 0;
            originalSenY = 0;
        }

        canDrop = true;
    }

    private void ThrowObject()
    {
        if (heldObj != null && heldObjRb != null)
        {
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
            heldObj.layer = 0;
            heldObjRb.isKinematic = false;
            heldObj.transform.parent = null;
            heldObjRb.AddForce(transform.forward * throwForce);
            heldObj = null;
        }
    }

    private void StopClipping()
    {
        if (heldObj == null) return;

        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
       
        if (hits.Length > 1)
        {
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); 
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from actions to prevent errors
        if (pickUpAction != null) pickUpAction.performed -= HandlePickUpDrop;
        if (throwAction != null) throwAction.performed -= HandleThrow;
        if (rotateAction != null)
        {
            rotateAction.performed -= StartRotation;
            rotateAction.canceled -= StopRotation;
        }
    }
}
