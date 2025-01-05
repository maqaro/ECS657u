using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, IDataPersistence
{
    private bool collected = false;
    public float healAmount = 20f; 
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    
    //An ID is created so each pickup is unique and once picked up is destroyed
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

    // This will load the saved data
    public void LoadData(GameData data)
    {
        data.healthPickupsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            gameObject.SetActive(false);
        }
    }

    // Will save the data to know which pickups have been picked up
    public void SaveData(ref GameData data)
    {
        if(collected)
        {
            if (data.healthPickupsCollected.ContainsKey(id))
            {
                data.healthPickupsCollected.Remove(id);
            }
            data.healthPickupsCollected.Add(id, collected);
        }
    }

    // User collision with the pickup
    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            //This checks if the player has the PlayerHealth Component
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                HealthCollected(playerHealth);
            }
        }
    }

    //Method for once the health pickup is collected
    private void HealthCollected(PlayerHealth playerHealth)
    {
        collected = true;
        playerHealth.Heal(healAmount);
        gameObject.SetActive(false); //Removes the gameobject once picked up
    }
}
