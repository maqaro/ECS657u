using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [Header("UI Elements")]
    [SerializeField] private RectTransform soundIndicatorUI; // VisualAudio panel
    [SerializeField] private Transform soundListContainer; // Parent for sound UI entries
    [SerializeField] private GameObject soundEntryPrefab; // Prefab for a single sound entry

    [Header("Player Settings")]
    [SerializeField] private Transform player; // Player's position
    [SerializeField] private float detectionRange = 10f; // Max range to detect sounds

    private Dictionary<AudioSource, GameObject> activeSounds = new Dictionary<AudioSource, GameObject>(); // Tracks active sounds and their UI entries

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        soundIndicatorUI.gameObject.SetActive(false); // Hide panel initially
        Debug.Log("SoundFXManager initialized and panel hidden.");
    }

    // -------------------
    // MONITORING SOUNDS
    // -------------------

    public void MonitorSound(AudioSource audioSource, Transform soundSource, string soundName)
    {
        if (audioSource == null || soundSource == null)
            return;

        // Skip if the sound is out of range
        if (Vector3.Distance(player.position, soundSource.position) > detectionRange)
        {
            RemoveSoundEntry(audioSource);
            return;
        }

        // If the sound is already being tracked, update its entry
        if (activeSounds.ContainsKey(audioSource))
        {
            UpdateSoundEntry(audioSource, soundSource.position, soundName);
        }
        else
        {
            // Add a new sound entry
            AddSoundEntry(audioSource, soundSource.position, soundName);
        }
    }

    private void AddSoundEntry(AudioSource audioSource, Vector3 soundPosition, string soundName)
    {
        // Create a new sound entry in the UI
        GameObject newSoundEntry = Instantiate(soundEntryPrefab, soundListContainer);

        // Get individual UI components for this sound entry
        TMP_Text soundText = newSoundEntry.transform.Find("SoundText")?.GetComponent<TMP_Text>();
        Image leftIndicator = newSoundEntry.transform.Find("IndicatorL")?.GetComponent<Image>();
        Image rightIndicator = newSoundEntry.transform.Find("IndicatorR")?.GetComponent<Image>();

        if (soundText == null || leftIndicator == null || rightIndicator == null)
        {
            Debug.LogError("AddSoundEntry: Missing components in Sound Entry Prefab. Please check the prefab setup.");
            Destroy(newSoundEntry);
            return;
        }

        // Assign the sound name
        soundText.text = soundName;

        // Insert the new sound entry at the top of the list
        newSoundEntry.transform.SetSiblingIndex(0);

        // Update indicators for this specific sound
        UpdateSoundIndicators(soundPosition, leftIndicator, rightIndicator);

        // Store the entry
        activeSounds[audioSource] = newSoundEntry;

        // Show the panel
        soundIndicatorUI.gameObject.SetActive(true);

        Debug.Log($"AddSoundEntry: Added sound '{soundName}'");
    }

    private void UpdateSoundEntry(AudioSource audioSource, Vector3 soundPosition, string soundName)
    {
        if (!activeSounds.ContainsKey(audioSource))
            return;

        // Get the UI elements for the sound entry
        GameObject soundEntry = activeSounds[audioSource];
        TMP_Text soundText = soundEntry.transform.Find("SoundText")?.GetComponent<TMP_Text>();
        Image leftIndicator = soundEntry.transform.Find("IndicatorL")?.GetComponent<Image>();
        Image rightIndicator = soundEntry.transform.Find("IndicatorR")?.GetComponent<Image>();

        if (soundText != null)
        {
            soundText.text = soundName;
        }

        // Update indicators
        UpdateSoundIndicators(soundPosition, leftIndicator, rightIndicator);

        Debug.Log($"UpdateSoundEntry: Updated sound '{soundName}' position.");
    }

    private void RemoveSoundEntry(AudioSource audioSource)
    {
        if (activeSounds.ContainsKey(audioSource))
        {
            Destroy(activeSounds[audioSource]); // Destroy the UI entry
            activeSounds.Remove(audioSource);

            Debug.Log("RemoveSoundEntry: Removed sound entry.");

            // Hide panel if no active sounds
            if (activeSounds.Count == 0)
            {
                soundIndicatorUI.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Check for stopped or out-of-range sounds
        List<AudioSource> toRemove = new List<AudioSource>();

        foreach (var sound in activeSounds)
        {
            if (sound.Key == null || !sound.Key.isPlaying || Vector3.Distance(player.position, sound.Key.transform.position) > detectionRange)
            {
                toRemove.Add(sound.Key);
            }
        }

        foreach (var audioSource in toRemove)
        {
            RemoveSoundEntry(audioSource);
        }
    }

    // -------------------
    // INDICATOR LOGIC
    // -------------------

    private void UpdateSoundIndicators(Vector3 soundPosition, Image leftIndicator, Image rightIndicator)
    {
        // Get the player's camera and calculate the sound's position relative to the player
        Transform playerCamera = Camera.main.transform;
        Vector3 directionToSound = soundPosition - playerCamera.position;
        float dotProduct = Vector3.Dot(playerCamera.right, directionToSound.normalized);

        // Handle indicators for this specific sound entry
        if (dotProduct > 0)
        {
            // Sound is on the right
            rightIndicator.color = new Color(1, 1, 1, 1); // Turn on right indicator
            leftIndicator.color = new Color(1, 1, 1, 0);  // Turn off left indicator
        }
        else
        {
            // Sound is on the left
            leftIndicator.color = new Color(1, 1, 1, 1);  // Turn on left indicator
            rightIndicator.color = new Color(1, 1, 1, 0); // Turn off right indicator
        }
    }

    // -------------------
    // OLD METHODS
    // -------------------

    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume, string soundName)
    {
        Debug.Log($"PlaySoundFXClip: Playing sound '{soundName}' at {spawnTransform.position}");

        AudioSource audioSource = Instantiate(new GameObject("TempAudioSource").AddComponent<AudioSource>(), spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Ensure spatial audio
        audioSource.Play();

        AddSoundEntry(audioSource, spawnTransform.position, soundName);

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("PlayRandomSoundFXClip: No audio clips provided!");
            return;
        }

        int rand = Random.Range(0, clips.Length);
        string soundName = clips[rand].name;

        Debug.Log($"PlayRandomSoundFXClip: Playing random sound '{soundName}' at {spawnTransform.position}");

        AudioSource audioSource = Instantiate(new GameObject("TempAudioSource").AddComponent<AudioSource>(), spawnTransform.position, Quaternion.identity);
        audioSource.clip = clips[rand];
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Ensure spatial audio
        audioSource.Play();

        AddSoundEntry(audioSource, spawnTransform.position, soundName);

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
