using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;

    private Rigidbody rb;

    private bool targetHit;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit)
        {
            return;
        }
        else
            targetHit = true;

        // Check if the projectile hit an enemy
        if (collision.gameObject.GetComponent<TargetEnemy>() != null)
        {
            TargetEnemy enemy = collision.gameObject.GetComponent<TargetEnemy>();

            enemy.TakeDamage(damage);

            Destroy(gameObject);

        }

        // Check if the projectile hit an extendable platform
        ExtendablePlatform platform = collision.gameObject.GetComponent<ExtendablePlatform>();
        if (platform != null)
        {
            platform.ExtendPlatform();
        }

        // Check if the projectile hit a platform trigger
        PlatformTrigger trigger = collision.gameObject.GetComponent<PlatformTrigger>();
        if (trigger != null)
        {
            foreach (var plat in trigger.platforms)
            {
                var extendable = plat.GetComponent<ExtendablePlatform>();
                if (extendable != null)
                {
                    extendable.TogglePlatform();
                }
            }
        }
    }
}