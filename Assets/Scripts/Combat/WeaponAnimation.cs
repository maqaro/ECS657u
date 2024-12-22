using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement pm;
    public InputActionAsset inputActionAsset;
    private InputAction MoveAction;
    private InputAction SprintAction;
    

    private void Start() {
        
        GetReferences();
        
    }

    private void Update() {
        UpdateSpeed();
    }

    private void GetReferences() 
    {
        animator = GetComponentInChildren<Animator>();
        pm = GetComponent<PlayerMovement>();
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
