using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float damage = 10f; // Default damage value
    private SwordSwing swordSwing;

    // Component references
    private Rigidbody rb;
    private bool targetHit;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Get the SwordSwing component from the player
        // Try to get the SwordSwing component, but don't rely on it
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            swordSwing = player.GetComponent<SwordSwing>();
            if (swordSwing != null)
            {
                damage = swordSwing.damage; // Use the sword's damage if available
            }
        }
    }

    // Called when this projectile collides with another collider
    private void OnCollisionEnter(Collision collision)
    {
        // Prevent multiple collision responses
        if (targetHit) return;
        targetHit = true;

        // Handle different types of collisions
        HandleEnemyHit(collision);
        HandleTriggerHit(collision);
        HandleTargetHit(collision);
    }

    // Stops the projectile's movement and physics
    private void StopProjectile(Collision collision)
    {
        if (rb == null) return;

        rb.isKinematic = true;
        transform.SetParent(collision.transform);
    }

    // Handles collision with enemy targets
    private void HandleEnemyHit(Collision collision)
    {
        EnemyAi enemy = collision.gameObject.GetComponent<EnemyAi>();
        if (enemy != null) // if the collided object is an enemy
        {
            // Use the damage variable directly instead of accessing through swordSwing
            enemy.health -= damage;
            if (enemy.health <= 0)
            {
                enemy.health = 0;
            }

        }
    }

    // Handles collision with the targets
    private void HandleTargetHit(Collision collision)
    {
        TargetEnemy target = collision.gameObject.GetComponent<TargetEnemy>();
        if (target != null) // if the collided object is a target
        {
            target.TakeDamage((int)damage);
            Destroy(gameObject); // Destroys the target once enough damage is done  
        }
    }

    // Handles collision with platform triggers
    private void HandleTriggerHit(Collision collision)
    {
        PlatformTrigger trigger = collision.gameObject.GetComponent<PlatformTrigger>();
        
        if (trigger == null) return;
        StopProjectile(collision);

        foreach (var plat in trigger.platforms)
        {
            var extendable = plat.GetComponent<ExtendablePlatform>();
            extendable?.TogglePlatform();
        }
    }
}