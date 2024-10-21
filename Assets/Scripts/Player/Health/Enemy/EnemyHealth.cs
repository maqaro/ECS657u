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

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (slider.value != currentHealth)
        {
            slider.value = currentHealth;
        }

        if (slider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, currentHealth, smoothSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (checkCollision(other))
        {
            TakeDamage(10);
        }
    }

    public bool checkCollision(Collider other)
    {
        if (other.gameObject.tag.Equals("Sword"))
        {
            return true;
        }
            
        return false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }
}