using TMPro; // Required for TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [Header("UI Elements")]
    [SerializeField] private RectTransform soundIndicatorUI; // VisualAudio panel (entire panel)
    [SerializeField] private Image indicatorLeft; // Left indicator image
    [SerializeField] private Image indicatorRight; // Right indicator image
    [SerializeField] private TMP_Text soundText; // Text object to display sound name

    [Header("Player Settings")]
    [SerializeField] private Transform player; // Player's position (main camera or player object)
    [SerializeField] private float detectionRange = 10f; // Max range to detect sounds

    private AudioSource currentAudioSource; // AudioSource currently being monitored
    private Transform currentSoundSource; // Transform of the current sound's source

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        soundIndicatorUI.gameObject.SetActive(false); // Hide panel initially
        Debug.Log("SoundFXManager initialized and panel hidden.");
    }

    /// <summary>
    /// Monitors an AudioSource and updates the UI if the sound is detected.
    /// </summary>
    public void MonitorSound(AudioSource audioSource, Transform soundSource, string soundName)
    {
        currentAudioSource = audioSource;
        currentSoundSource = soundSource;

        if (audioSource.isPlaying)
        {
            Debug.Log($"MonitorSound: Detected sound '{soundName}' at position {soundSource.position}");
            UpdateSoundIndicators(soundSource.position, soundName);
        }
        else
        {
            HidePanel();
        }
    }

    /// <summary>
    /// Plays a specific sound and updates the UI.
    /// </summary>
    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume, string soundName)
    {
        Debug.Log($"PlaySoundFXClip: Playing sound '{soundName}' at {spawnTransform.position}");
        
        // Play audio
        AudioSource audioSource = Instantiate(new GameObject("TempAudioSource").AddComponent<AudioSource>(), spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Ensure spatial audio
        audioSource.Play();

        // Update UI elements
        UpdateSoundIndicators(spawnTransform.position, soundName);

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    /// <summary>
    /// Plays a random sound from a list and updates the UI.
    /// </summary>
    public void PlayRandomSoundFXClip(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("PlayRandomSoundFXClip: No audio clips provided!");
            return;
        }

        int rand = Random.Range(0, clips.Length); // Select a random clip
        string soundName = clips[rand].name;

        Debug.Log($"PlayRandomSoundFXClip: Playing random sound '{soundName}' at {spawnTransform.position}");

        // Play audio
        AudioSource audioSource = Instantiate(new GameObject("TempAudioSource").AddComponent<AudioSource>(), spawnTransform.position, Quaternion.identity);
        audioSource.clip = clips[rand];
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Ensure spatial audio for 3D effects
        audioSource.Play();

        // Update UI elements
        UpdateSoundIndicators(spawnTransform.position, soundName);

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    private void Update()
    {
        // Continuously check if a monitored sound is within range
        if (currentAudioSource != null && currentSoundSource != null)
        {
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

    /// <summary>
    /// Updates the sound indicators (UI) based on the sound's position relative to the player.
    /// </summary>
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
        Debug.Log($"UpdateSoundIndicators: Displaying sound '{soundName}'");

        if (dotProduct > 0)
        {
            // Sound is on the right
            indicatorRight.color = new Color(1, 1, 1, 1); // Turn on right indicator
            indicatorLeft.color = new Color(1, 1, 1, 0);  // Turn off left indicator
            Debug.Log("UpdateSoundIndicators: Right indicator activated.");
        }
        else
        {
            // Sound is on the left
            indicatorLeft.color = new Color(1, 1, 1, 1);  // Turn on left indicator
            indicatorRight.color = new Color(1, 1, 1, 0); // Turn off right indicator
            Debug.Log("UpdateSoundIndicators: Left indicator activated.");
        }
    }

    /// <summary>
    /// Hides the sound indicator UI panel and resets the indicators.
    /// </summary>
    private void HidePanel()
    {
        // Hide the entire panel
        soundIndicatorUI.gameObject.SetActive(false);
        Debug.Log("HidePanel: Panel hidden.");

        // Clear the indicators and sound text
        indicatorLeft.color = new Color(1, 1, 1, 0);
        indicatorRight.color = new Color(1, 1, 1, 0);
        soundText.text = "";
        Debug.Log("HidePanel: Indicators and sound text cleared.");
    }
}
