using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold; // Threshold for falling below the map
    [SerializeField] private string respawnTag = "RespawnPoint"; // Tag to find respawn point
    private Transform respawnPoint; // The respawn point transform
    private PlayerHealth playerHealth; // Reference to player's health script

    void Start()
    {
        // Find the respawn point object
        GameObject respawnObj = GameObject.FindGameObjectWithTag(respawnTag);
        if (respawnObj != null)
        {
            // Set the respawn point transform
            respawnPoint = respawnObj.transform;
        }
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Check if the player has fallen below the map or has no health
        if ((playerHealth != null && playerHealth.currentHealth <= 0) || transform.position.y < threshold)
        {
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        // Check if we have a valid respawn point
        if (respawnPoint != null)
        {
            // Set the player's position to the respawn point
            Vector3 adjustedRespawnPosition = respawnPoint.position;
            adjustedRespawnPosition.y += 1.0f; 

            transform.position = adjustedRespawnPosition;

            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                // Reset the player's health to the max health
                playerHealth.currentHealth = playerHealth.maxHealth;
                playerHealth.healthbar.SetHealth(playerHealth.currentHealth);
            }

            // Reset the player's velocity
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Reset the player's velocity
                rb.velocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero; 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has collided with a checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            // Set the respawn point to the checkpoint
            respawnPoint = other.transform;

            Renderer checkpointRenderer = other.GetComponent<Renderer>();
            Collider checkpointCollider = other.GetComponent<Collider>();

            // Disable the checkpoint renderer and collider
            if (checkpointRenderer != null)
            {
                checkpointRenderer.enabled = false;
            }

            if (checkpointCollider != null)
            {
                checkpointCollider.enabled = false;
            }

        }
    }
}
