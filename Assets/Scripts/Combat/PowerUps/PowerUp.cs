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
            GenerateId();
        }
    }

    public void LoadData(GameData data)
    {
        data.powerUpsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (collected)
        {
            if (data.powerUpsCollected.ContainsKey(id))
            {
                data.powerUpsCollected.Remove(id);
            }
            data.powerUpsCollected.Add(id, collected);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                ApplyPowerUp(playerStats);
            }
        }
    }

    private void ApplyPowerUp(PlayerStats playerStats)
    {
        collected = true;
        playerStats.ActivatePowerUp(powerUpType, multiplier, duration);
        gameObject.SetActive(false);
    }
}