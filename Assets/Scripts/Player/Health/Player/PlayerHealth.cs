using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDataPersistence
{
    [SerializeField] private AudioClip[] hitSounds;
    public float maxHealth = 100f;
    public float currentHealth;
    public Healthbar healthbar;
    private bool hasLoadedData = false;

    void Start()
    {
        // Only set to max health if we haven't loaded data
        if (!hasLoadedData)
        {
            currentHealth = maxHealth;
            healthbar.SetMaxHealth(maxHealth);
        }
    }

    public void LoadData(GameData data)
    {
        hasLoadedData = true;
        this.currentHealth = data.health;
        if (healthbar != null)
        {
            healthbar.SetMaxHealth(maxHealth);
            healthbar.SetHealth(currentHealth);
        }
    }

    public void SaveData(ref GameData data)
    {
        // Save the player's health to the data
        data.health = this.currentHealth;
    }

    public void TakeDamage(float damage)
    {
        SoundFXManager.instance.PlayRandomSoundFXClip(hitSounds, transform, 0.3f);
        // Reduce the player's health by the damage amount
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // If the player's health is less than or equal to 0, set the health to 0
            currentHealth = 0;
        }
        healthbar.SetHealth(currentHealth);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Update the health bar
        healthbar.SetHealth(currentHealth);
    }
}
