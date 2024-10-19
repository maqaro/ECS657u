using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("HoldLayer"); 
        swordAnimator = sword.GetComponent<Animator>();
        PlayerCamScript = playerCamera.GetComponent<PlayerCam>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (heldObj == null) 
            {
                RaycastHit hit;
                float sphereRadius = 0.5f;
                
                if (Physics.SphereCast(playerCamera.transform.position, sphereRadius, playerCamera.transform.forward, out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if(canDrop == true)
                {
                    StopClipping(); 
                    DropObject();
                }
            }
        }
        if (heldObj != null) 
        {
            MoveObject(); 
            RotateObject();
            if (Input.GetKeyDown(KeyCode.Mouse1) && canDrop == true) 
            {
                StopClipping();
                ThrowObject();
            }

        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) 
        {
            swordAnimator.enabled = false;
            sword.SetActive(false);
            
            heldObj = pickUpObj; 
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            heldObj.layer = LayerNumber; 

            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0; 
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; 
        heldObj = null; 

        sword.SetActive(true);
        swordAnimator.enabled = true;
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R))
        {
            canDrop = false; 

            if (originalSenX == 0 && originalSenY == 0)
            {
                originalSenX = PlayerCamScript.senX;
                originalSenY = PlayerCamScript.senY;
            }

            PlayerCamScript.senX = 0f;
            PlayerCamScript.senY = 0f;

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;

            heldObj.transform.Rotate(Vector3.down, XaxisRotation);   
            heldObj.transform.Rotate(Vector3.right, YaxisRotation); 
        }
        else
        {
            if (originalSenX != 0 && originalSenY != 0)
            {
                PlayerCamScript.senX = originalSenX;
                PlayerCamScript.senY = originalSenY;

                originalSenX = 0;
                originalSenY = 0;
            }

            canDrop = true;
        }
    }


    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
        
        sword.SetActive(true);
        swordAnimator.enabled = true;
    }

    void StopClipping()
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