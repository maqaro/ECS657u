using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumScript : MonoBehaviour
{
    public float speed = 1.5f;
    public float limit = 40f;
    public bool randomStart = false;
    private float random = 0;

    [Header("Knockback Settings")]
    public float knockbackForce = 20f;  // Force of knockback applied to player

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

    // Detect collision with player
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided is the player (you may need to adjust the tag accordingly)
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Calculate knockback direction (from blade to player)
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                knockbackDirection.y = 0.5f;  // Optional: Add some upward force to the knockback

                // Apply knockback force
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}
