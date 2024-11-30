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
        {
            targetHit = true;
        }

        //This will stop the projectiles movemtent once it hits something
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Make the projectile a child of the object it hit
        transform.SetParent(collision.transform);
        // this will set the position of the projectile to the exact point of contact with the collider
        transform.position = collision.contacts[0].point;

        // Check if the projectile hit an enemy
        if (collision.gameObject.GetComponent<TargetEnemy>() != null)
        {
            TargetEnemy enemy = collision.gameObject.GetComponent<TargetEnemy>();
            enemy.TakeDamage(damage);

        }

        // Check if the projectile hit an extendable platform
        ExtendablePlatform platform = collision.gameObject.GetComponent<ExtendablePlatform>();
        platform?.ExtendPlatform();

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