using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // How much damage this projectile deals to enemies
    public int damage = 10;

    // Component references
    private Rigidbody rb;
    private bool targetHit;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called when this projectile collides with another collider
    private void OnCollisionEnter(Collision collision)
    {
        // Prevent multiple collision responses
        if (targetHit) return;
        targetHit = true;

        // Stop the projectile's movement
        StopProjectile();

        // Handle different types of collisions
        HandleEnemyHit(collision);
        HandleTriggerHit(collision);
    }

    // Stops the projectile's movement and physics
    private void StopProjectile()
    {
        if (rb == null) return;

        rb.velocity = Vector3.zero;
    }

    // Handles collision with enemy targets
    private void HandleEnemyHit(Collision collision)
    {
        TargetEnemy enemy = collision.gameObject.GetComponent<TargetEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

    // Handles collision with platform triggers
    private void HandleTriggerHit(Collision collision)
    {
        PlatformTrigger trigger = collision.gameObject.GetComponent<PlatformTrigger>();
        if (trigger == null) return;

        foreach (var plat in trigger.platforms)
        {
            var extendable = plat.GetComponent<ExtendablePlatform>();
            extendable?.TogglePlatform();
        }
    }
}