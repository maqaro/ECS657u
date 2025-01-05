using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour, IDataPersistence
{
    [Header("Power-up Settings")]
    public PowerUpType powerUpType;
    public float multiplier;
    public float duration;
    
    private bool collected = false;
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]

     private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(id))
        {
            GenerateId(); //Generates a unique id for each pickup
        }
    }

    // Check if powerup has already been picked up and set as active if it has been picked up
    public void LoadData(GameData data)
    {
        data.powerUpsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    // Saves any data in the case that the user has collected a powerup
    public void SaveData(ref GameData data)
    {
        if (collected)
        {
            if (data.powerUpsCollected.ContainsKey(id))
            {
                data.powerUpsCollected.Remove(id);
            }
            data.powerUpsCollected.Add(id, collected); // Set the status as collected
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.CompareTag("Player")) //Check if the collision is by the tag 'Player'
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>(); //find 'PlayerStats' in the player component
            if (playerStats != null)
            {
                ApplyPowerUp(playerStats); // will apply powerup
            }
        }
    }

    private void ApplyPowerUp(PlayerStats playerStats)
    {
        collected = true;
        playerStats.ActivatePowerUp(powerUpType, multiplier, duration);
        gameObject.SetActive(false); // will destroy the powerup once picked up
    }
}