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

    public void SaveData(ref GameData data)
    {
        if (data.kunaiPickupsCollected.ContainsKey(id))
        {
            data.kunaiPickupsCollected.Remove(id);
        }
        data.kunaiPickupsCollected.Add(id, collected);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            Throwing throwingScript = other.GetComponent<Throwing>();

            if (throwingScript != null)
            {
                KunaiCollected(throwingScript);
            }
        }
    }

    private void KunaiCollected(Throwing throwingScript)
    {
        collected = true;
        throwingScript.AddThrows(kunaiAmount);
        gameObject.SetActive(false);
    }
}
