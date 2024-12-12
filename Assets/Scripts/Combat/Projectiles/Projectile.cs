using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // How much damage this projectile deals to enemies
    public int damage;

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

        // Handle different types of collisions
        HandleEnemyHit(collision);
        HandleTriggerHit(collision);
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
        TargetEnemy enemy = collision.gameObject.GetComponent<TargetEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
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