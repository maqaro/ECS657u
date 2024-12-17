using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // For the new Input System

public class Throwing : MonoBehaviour
{
    // References for throwing logic
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    // References for throwing logic
    public int totalThrows;
    public float throwCooldown = 1f;
    public float throwForce = 20f;
    public float throwUpwardForce = 2f;
    public bool showCooldownDebug = true;
    
    // Show amount of time left before you can throw another kunai
    private float nextThrowTime = 0f;
    private bool readyToThrow = true;

    // New Input System
    private Contols playerControls;
    private InputAction throwAction;
    public GameObject pickUpHolder;

    private void Awake()
    {
        // Initialize the new Input System controls
        playerControls = new Contols();
    }

    private void OnEnable()
    { 
        // Enable the Throw action
        throwAction = playerControls.Player.Throw;
        throwAction.Enable();
        throwAction.performed += HandleThrow;
    }

    private void OnDisable()
    {
        // Disable the Throw action
        throwAction.Disable();
        throwAction.performed -= HandleThrow;
    }

    private void Start()
    {
        readyToThrow = true;
    }

    private void Update()
    {
        
    }

    private void HandleThrow(InputAction.CallbackContext context)
    {
        // Check if enough time has passed since last throw
        if (Time.time < nextThrowTime) 
        {
            if (showCooldownDebug)
            {
                Debug.Log($"Throw on cooldown. Ready in {nextThrowTime - Time.time:F1} seconds");
            }
            return;
        }

        // Trigger the throw if the player is ready and has remaining throws
        if (readyToThrow && totalThrows > 0 && pickUpHolder.transform.childCount == 0)
        {
            Throw();
            nextThrowTime = Time.time + throwCooldown;
        }
    }

    private void Throw()
    {
        readyToThrow = false;

        // Instantiate the object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        Destroy(projectile, 5f);
        totalThrows--;

        // Reset throw cooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }

    public void AddThrows(int amount)
    {
        totalThrows += amount;
        Debug.Log($"Kunai picked up! Total throws: {totalThrows}");
    }
}
