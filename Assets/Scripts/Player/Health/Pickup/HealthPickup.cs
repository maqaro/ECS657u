using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, IDataPersistence
{
    private bool collected = false;
    public float healAmount = 20f; 
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        // If no ID has been assigned in the editor, generate one
        if (string.IsNullOrEmpty(id))
        {
            GenerateId();
        }
    }

    public void LoadData(GameData data)
    {
        data.healthPickupsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.healthPickupsCollected.ContainsKey(id))
        {
            data.healthPickupsCollected.Remove(id);
        }
        data.healthPickupsCollected.Add(id, collected);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                HealthCollected(playerHealth);
            }
        }
    }

    private void HealthCollected(PlayerHealth playerHealth)
    {
        collected = true;
        playerHealth.Heal(healAmount);
        gameObject.SetActive(false);
    }
}
