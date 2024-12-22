using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement pm;
    private PickUpScript pu;
    public InputActionAsset inputActionAsset;
    private InputAction MoveAction;
    private InputAction SprintAction;
    

    private void Start() {
        GetReferences();
    }

    private void Update() {
        UpdateSpeed();
        heldObject();
        Debug.Log($"WeaponUp: {animator.GetBool("WeaponUp")}, Up: {animator.GetFloat("Up")}");
    }

    private void GetReferences() 
    {
        animator = GetComponentInChildren<Animator>();
        pm = GetComponent<PlayerMovement>();
        pu = GetComponent<PickUpScript>();
        var gameplayActionMap = inputActionAsset.FindActionMap("Player");

        if (gameplayActionMap != null)
        {
            // Get movement and sprint actions
            MoveAction = gameplayActionMap.FindAction("Move");
            SprintAction = gameplayActionMap.FindAction("Sprint");

            // Enable the actions
            SprintAction?.Enable();
            MoveAction?.Enable();

        }
        
    }

    private void heldObject()
    {
        if (pu.isHolding)
        {
            animator.SetBool("WeaponUp", false);
            animator.SetFloat("Up", 0f);
        }
        else
        {
            animator.SetBool("WeaponUp", true);
            animator.SetFloat("Up", 1f);
        }
    }

    private void UpdateSpeed()
    {
        if (pm.moveInput == Vector2.zero || pm.state ==  PlayerMovement.MovementState.crouching)
        {
            animator.SetFloat("Speed", 0f, 0.2f, Time.deltaTime);
        }
        else if (pm.state == PlayerMovement.MovementState.sprinting)
        {
            animator.SetFloat("Speed", 1f, 0.2f, Time.deltaTime);
        }
        else if (pm.state == PlayerMovement.MovementState.walking)
        {
            animator.SetFloat("Speed", 0.5f, 0.2f, Time.deltaTime);
        }
    }
}
