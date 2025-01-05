using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // How much damage this projectile deals to enemies
    private SwordSwing swordSwing;

    // Component references
    private Rigidbody rb;
    private bool targetHit;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Get the SwordSwing component from the player
        swordSwing = GameObject.FindGameObjectWithTag("Player").GetComponent<SwordSwing>();
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
            // Apply damage to the enemy
            enemy.health -= swordSwing.damage;
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
            target.TakeDamage((int)swordSwing.damage);
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