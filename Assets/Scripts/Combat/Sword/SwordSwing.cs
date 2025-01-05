using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SwordSwing : MonoBehaviour
{
    [Header("References")]
    public GameObject Sword; 
    public Image cooldownImage; 
    public InputActionAsset inputActionAsset;

    [Header("Settings")]
    public bool canAttack = true; 
    public float Cooldown = 1.0f; 
    public float attackRange = 1.5f; 
    public float damage = 10f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip[] swingSounds;

    private InputAction swordSwingAction;
    private WeaponAnimation wa;
    private Animator animator;

    void OnEnable()
    {
        // Get the "SwordSwing" input action
        var gameplayActionMap = inputActionAsset.FindActionMap("Player");
        swordSwingAction = gameplayActionMap.FindAction("SwordSwing");
        swordSwingAction.performed += ctx => OnSwordSwing(); // Add a listener for when the action is performed
        swordSwingAction.Enable();

        // Initialize components
        wa = GetComponent<WeaponAnimation>();
        animator = GetComponentInChildren<Animator>();

        // Initialize the cooldown fill image as empty
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; 
        }
    }

    void OnDisable()
    {
        if (swordSwingAction != null)
        {
            swordSwingAction.performed -= ctx => OnSwordSwing();
            swordSwingAction.Disable();
        }
    }

    // Method that triggers when the sword swing action is performed
    private void OnSwordSwing()
    {
        if (canAttack && animator.GetBool("WeaponUp"))
        {
            Attack();
            
            wa.SwingAnimation(); // Play swing animation
        }
    }

    // Method that triggers when the sword swing action is performed
    private void Attack()
    {
        canAttack = false;
        SoundFXManager.instance.PlayRandomSfxPlayer(swingSounds, transform, 0.1f);
        // Check for enemies in the attack range
        Collider[] hitColliders = Physics.OverlapSphere(Sword.transform.position, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            // If the collider is an enemy, apply damage
            if (hitCollider.CompareTag("Enemy"))
            {
                // Get the EnemyAi script and apply damage
                EnemyAi enemyScript = hitCollider.GetComponent<EnemyAi>();
                if (enemyScript != null)
                {
                    // Apply damage to the enemy
                    enemyScript.health -= damage;
                    if (enemyScript.health <= 0)
                    {
                        enemyScript.health = 0;
                    }
                }
            }
        }

        // Start the cooldown coroutine
        StartCoroutine(ResetAttackCooldown());
    }

    // Coroutine to reset the attack cooldown
    private IEnumerator ResetAttackCooldown()
    {
        float elapsed = 0f;

        // Update the slider gradually
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f; // Start full
        }

        while (elapsed < Cooldown)
        {
            elapsed += Time.deltaTime;

            // Update Slider's value
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 1f - (elapsed / Cooldown);
            }

            yield return null;
        }

        // Reset cooldown state
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // Set overlay to empty
        }

        canAttack = true; // Allow attacking again
    }
}
