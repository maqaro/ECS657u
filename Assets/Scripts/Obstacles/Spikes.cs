using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public float damage = 100f; // Damage dealt by the spikes

    // Detect collision with the player
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object colliding with the spikes has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the PlayerHealth script component from the player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            
            if (playerHealth != null)
            {
                // Decrease the player's health
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
