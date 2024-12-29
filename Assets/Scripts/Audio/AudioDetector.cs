using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioDetector : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // Reference to the player
    [SerializeField] private Canvas soundCanvas; // Reference to the Canvas for visual indicators
    [SerializeField] private GameObject soundIndicatorPrefab; // Prefab for the sound indicator
    [SerializeField] private float indicatorDuration = 2f; // Duration the indicator stays visible

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in AudioDetector.");
        }

        if (soundCanvas == null)
        {
            Debug.LogError("Sound Canvas is not assigned in AudioDetector.");
        }
    }

    public void DetectAndVisualizeSound(Transform soundSource, string soundName)
    {
        // Calculate the direction of the sound relative to the player
        Vector3 direction = soundSource.position - playerTransform.position;
        direction.y = 0; // Ignore vertical differences for simplicity
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        // Create the sound indicator
        GameObject indicator = Instantiate(soundIndicatorPrefab, soundCanvas.transform);

        // Update indicator text
        TextMeshProUGUI textComponent = indicator.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = soundName;
        }

        // Rotate the indicator to match the direction
        RectTransform indicatorRect = indicator.GetComponent<RectTransform>();
        indicatorRect.rotation = Quaternion.Euler(0, 0, -angle);

        // Destroy the indicator after a set duration
        Destroy(indicator, indicatorDuration);
    }
}
