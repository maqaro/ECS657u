using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
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
}
