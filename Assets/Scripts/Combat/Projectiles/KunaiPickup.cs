using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiPickup : MonoBehaviour, IDataPersistence
{
    private bool collected = false;
    public int kunaiAmount = 20; // Number of kunai this pickup adds

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
        // Try to get the collected status from saved data
        data.kunaiPickupsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            // If this kunai was already collected, disable the GameObject
            gameObject.SetActive(false);
        }
    }

    // Saves any data in the case that the user has collected a pickup
    public void SaveData(ref GameData data)
    {
        if(collected)
        {
            if (data.kunaiPickupsCollected.ContainsKey(id)) // Removes an exisiting entry with the same ID
            {
                data.kunaiPickupsCollected.Remove(id);
            }
            data.kunaiPickupsCollected.Add(id, collected); //Set the status as collected
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (!collected && other.CompareTag("Player"))  //Check if the collision is by the tag 'Player'
        {
            Throwing throwingScript = other.GetComponent<Throwing>(); //find 'Throwing' in the player component

            if (throwingScript != null)
            {
                KunaiCollected(throwingScript); //will add the kunai to the current user balance
            }
        }
    }

    // Method for after the pickup is collected
    private void KunaiCollected(Throwing throwingScript)
    {
        collected = true;
        throwingScript.AddThrows(kunaiAmount); // will add throws 
        gameObject.SetActive(false); // will destroy the pickup once collected
    }
}
