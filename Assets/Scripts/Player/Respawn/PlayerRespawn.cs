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
        GameObject respawnObj = GameObject.FindGameObjectWithTag(respawnTag);
        if (respawnObj != null)
        {
            respawnPoint = respawnObj.transform;
        }
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
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
            Vector3 adjustedRespawnPosition = respawnPoint.position;
            adjustedRespawnPosition.y += 1.0f; 

            transform.position = adjustedRespawnPosition;

            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.currentHealth = playerHealth.maxHealth;
                playerHealth.healthbar.SetHealth(playerHealth.currentHealth);
            }

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero; 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            respawnPoint = other.transform;

            Renderer checkpointRenderer = other.GetComponent<Renderer>();
            Collider checkpointCollider = other.GetComponent<Collider>();

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
