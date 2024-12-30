using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public Slider slider;
    public Slider easeSlider;
    public float maxHealth = 100;
    public float currentHealth = 100;
    private float smoothSpeed = 0.05f;
    public SwordSwing swordSwing; 

    void Start()
    {
        // Set the slider's max value to the max health
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Update the slider's value to the current health
        if (slider.value != currentHealth)
        {
            slider.value = currentHealth;
        }

        // Update the ease slider's value to the current health
        if (slider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, currentHealth, smoothSpeed);
        }
    }

    // If the enemy collides with the player's sword, take damage
    void OnTriggerEnter(Collider other)
    {
        if (checkCollision(other))
        {
            TakeDamage(swordSwing.damage);
        }
    }

    // Check if the enemy collides with the player's sword
    public bool checkCollision(Collider other)
    {
        if (other.gameObject.tag.Equals("Sword"))
        {
            return true;
        }
            
        return false;
    }

    // Reduce the enemy's health by the damage amount
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }
}