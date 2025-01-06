using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumScript : MonoBehaviour
{
    //Variables defined for the pendulum
    public float speed = 1.5f;
    public float limit = 40f;
    public bool randomStart = false;
    private float random = 0;

    //Attempt to fix the pendulum with knockback settings needs to be fixed
    [Header("Knockback Settings")]
    public float knockbackForce = 20f;

    void Awake()
    {
        if (randomStart)
            random = Random.Range(0f, 1f);
    }

    void Update()
    {
        float angle = limit * Mathf.Sin(Time.time * speed);
        transform.localRotation = Quaternion.Euler(0, 90, angle);
    }

    // Detect Any collision with the player
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided has the player layer attached
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Calculate knockback direction (from blade to player)
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                knockbackDirection.y = 0.5f; //This knockback damage needs to be added


                //Add the Knockback Force, (needs to be fixed)
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}
