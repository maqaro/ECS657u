using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDataPersistence
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Healthbar healthbar;

    void Start()
    {
        // Set the player's current health to the max health
        currentHealth = maxHealth;
        healthbar.SetMaxHealth(maxHealth);
    }

    public void LoadData(GameData data)
    {
        // Load the player's health from the saved data
        this.currentHealth = data.health;
        // Update the healthbar to reflect the loaded health
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
