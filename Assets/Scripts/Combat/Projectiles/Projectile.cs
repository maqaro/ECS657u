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

        if (collision.gameObject.GetComponent<TargetEnemy>() != null)
        {
            TargetEnemy enemy = collision.gameObject.GetComponent<TargetEnemy>();

            enemy.TakeDamage(damage);

            Destroy(gameObject);

        }
    }
}
