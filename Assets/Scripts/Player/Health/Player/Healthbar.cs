using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Slider easeSlider;
    public float maxHealth = 100;
    public float currentHealth = 100;
    private float smoothSpeed = 0.01f;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        easeSlider.maxValue = maxHealth;
        easeSlider.value = currentHealth;
    }

    void Update()
    {
        if (slider.value != currentHealth)
        {
            slider.value = currentHealth;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }

        if (slider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, currentHealth, smoothSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }

        slider.value = currentHealth;
    }
}