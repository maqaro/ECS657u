using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundIndicatorUI : MonoBehaviour
{
    [SerializeField] private Text soundNameText;
    [SerializeField] private RectTransform arrow;
    [SerializeField] private float indicatorLifetime = 5f; // Default duration for the indicator

    private Vector3 soundWorldPosition;
    private Transform playerCamera;
    private float remainingLifetime;

    public event Action OnIndicatorRemoved;

    public void Initialise(Vector3 position, string soundName)
    {
        soundWorldPosition = position;
        soundNameText.text = soundName;
        remainingLifetime = indicatorLifetime;

        playerCamera = Camera.main.transform; // Reference the player's main camera
    }

    private void Update()
    {
        // Update Lifetime
        remainingLifetime -= Time.deltaTime;
        if (remainingLifetime <= 0)
        {
            OnIndicatorRemoved?.Invoke();
            Destroy(gameObject);
            return;
        }

        // Calculate direction to sound
        Vector3 directionToSound = soundWorldPosition - playerCamera.position;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(soundWorldPosition);

        // Update Arrow Direction
        float angle = Mathf.Atan2(directionToSound.x, directionToSound.z) * Mathf.Rad2Deg;
        arrow.localRotation = Quaternion.Euler(0, 0, -angle);

        // Update Position (Keep it in stacking logic)
        transform.localPosition = CalculateStackPosition();
    }

    private Vector3 CalculateStackPosition()
    {
        // Example: Stack vertically within the parent
        int index = transform.GetSiblingIndex();
        float verticalSpacing = 50f; // Adjust spacing as needed
        return new Vector3(0, -index * verticalSpacing, 0);
    }

    public void ExtendLifetime()
    {
        remainingLifetime = indicatorLifetime; // Reset or extend the lifetime
    }
}
