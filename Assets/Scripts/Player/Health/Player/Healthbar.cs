using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Slider easeSlider;
    private float smoothSpeed = 0.01f;

    void Start()
    {
        
    }

    void Update()
    {
        // Update the ease slider's value to the current health
        if (slider.value != easeSlider.value)
        {
            easeSlider.value = Mathf.Lerp(easeSlider.value, slider.value, smoothSpeed);
        }
    }

    public void SetMaxHealth(float health)
    {
        // Set the slider's max value to the max health
        slider.maxValue = health;
        easeSlider.maxValue = health;
        slider.value = health;
        easeSlider.value = health;
    }

    public void SetHealth(float health)
    {
        // Update the slider's value to the current health
        slider.value = health;
    }
}