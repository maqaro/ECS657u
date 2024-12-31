using TMPro; // Required for TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private RectTransform soundIndicatorUI; // VisualAudio panel (entire panel)
    [SerializeField] private Image indicatorLeft; // Left indicator image
    [SerializeField] private Image indicatorRight; // Right indicator image
    [SerializeField] private TMP_Text soundText; // Text object to display sound name
    [SerializeField] private Transform player; // Player's position (main camera or player object)
    [SerializeField] private float detectionRange = 10f; // Max range to detect sounds

    private AudioSource currentAudioSource;
    private Transform currentSoundSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        soundIndicatorUI.gameObject.SetActive(false); // Hide panel initially
    }

    public void MonitorSound(AudioSource audioSource, Transform soundSource, string soundName)
    {
        currentAudioSource = audioSource;
        currentSoundSource = soundSource;

        if (audioSource.isPlaying)
        {
            UpdateSoundIndicators(soundSource.position, soundName);
        }
        else
        {
            HidePanel();
        }
    }

    private void Update()
    {
        if (currentAudioSource != null && currentSoundSource != null)
        {
            // Check if the sound is within detection range
            float distance = Vector3.Distance(player.position, currentSoundSource.position);

            if (currentAudioSource.isPlaying && distance <= detectionRange)
            {
                UpdateSoundIndicators(currentSoundSource.position, currentAudioSource.clip.name);
            }
            else
            {
                HidePanel();
            }
        }
    }

    private void UpdateSoundIndicators(Vector3 soundPosition, string soundName)
    {
        // Show the panel
        soundIndicatorUI.gameObject.SetActive(true);

        // Get the player's camera and calculate the sound's position relative to the player
        Transform playerCamera = Camera.main.transform;
        Vector3 directionToSound = soundPosition - playerCamera.position;
        float dotProduct = Vector3.Dot(playerCamera.right, directionToSound.normalized);

        // Display the sound name
        soundText.text = soundName;

        if (dotProduct > 0)
        {
            // Sound is on the right
            indicatorRight.color = new Color(1, 1, 1, 1); // Turn on right indicator
            indicatorLeft.color = new Color(1, 1, 1, 0);  // Turn off left indicator
        }
        else
        {
            // Sound is on the left
            indicatorLeft.color = new Color(1, 1, 1, 1);  // Turn on left indicator
            indicatorRight.color = new Color(1, 1, 1, 0); // Turn off right indicator
        }
    }

    private void HidePanel()
    {
        // Hide the entire panel
        soundIndicatorUI.gameObject.SetActive(false);

        // Clear the indicators and sound text
        indicatorLeft.color = new Color(1, 1, 1, 0);
        indicatorRight.color = new Color(1, 1, 1, 0);
        soundText.text = "";
    }
}
