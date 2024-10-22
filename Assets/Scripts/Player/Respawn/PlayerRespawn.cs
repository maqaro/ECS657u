using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold; 
    [SerializeField] string respawnTag = "RespawnPoint";
    private Transform respawnPoint; 
    private PlayerHealth playerHealth; 

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
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;

            if (playerHealth != null && playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.currentHealth = playerHealth.maxHealth;
                playerHealth.healthbar.SetHealth(playerHealth.currentHealth);
            }
        }
    }
}
