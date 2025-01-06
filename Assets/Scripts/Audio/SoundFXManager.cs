using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [Header("Audio Mixer Groups")] 
    [SerializeField] private AudioMixerGroup soundFXMixerGroup;

    [Header("UI Elements")]
    [SerializeField] private RectTransform visualAudio;
    [SerializeField] private RectTransform soundListContainer;
    [SerializeField] private GameObject soundEntryPrefab;

    [Header("Player Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;

    private Dictionary<AudioSource, GameObject> activeSounds = new Dictionary<AudioSource, GameObject>();
    private float baseHeight = 0f;
    private float entryHeight = 50f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        visualAudio.gameObject.SetActive(false);
    }

    // -------------------
    // MONITORING SOUNDS
    // -------------------

    public void MonitorSound(AudioSource audioSource, Transform soundSource, string soundName)
    {
        if (audioSource == null || soundSource == null)
            return;

        if (Vector3.Distance(player.position, soundSource.position) > detectionRange)
        {
            RemoveSoundEntry(audioSource);
            return;
        }

        if (activeSounds.ContainsKey(audioSource))
        {
            UpdateSoundEntry(audioSource, soundSource.position, soundName);
        }
        else
        {
            AddSoundEntry(audioSource, soundSource.position, soundName);
        }
    }

    private void AddSoundEntry(AudioSource audioSource, Vector3 soundPosition, string soundName)
    {
        GameObject newSoundEntry = Instantiate(soundEntryPrefab, soundListContainer);

        TMP_Text soundText = newSoundEntry.transform.Find("SoundText")?.GetComponent<TMP_Text>();
        Image leftIndicator = newSoundEntry.transform.Find("IndicatorL")?.GetComponent<Image>();
        Image rightIndicator = newSoundEntry.transform.Find("IndicatorR")?.GetComponent<Image>();

        if (soundText == null || leftIndicator == null || rightIndicator == null)
        {
            Destroy(newSoundEntry);
            return;
        }

        soundText.text = soundName;
        newSoundEntry.transform.SetSiblingIndex(0);
        UpdateSoundIndicators(soundPosition, leftIndicator, rightIndicator);

        activeSounds[audioSource] = newSoundEntry;
        AdjustPanelSize();
        visualAudio.gameObject.SetActive(true);
    }

    private void UpdateSoundEntry(AudioSource audioSource, Vector3 soundPosition, string soundName)
    {
        if (!activeSounds.ContainsKey(audioSource)) return;

        GameObject soundEntry = activeSounds[audioSource];
        TMP_Text soundText = soundEntry.transform.Find("SoundText")?.GetComponent<TMP_Text>();
        Image leftIndicator = soundEntry.transform.Find("IndicatorL")?.GetComponent<Image>();
        Image rightIndicator = soundEntry.transform.Find("IndicatorR")?.GetComponent<Image>();

        if (soundText != null)
        {
            soundText.text = soundName;
        }

        UpdateSoundIndicators(soundPosition, leftIndicator, rightIndicator);
    }

    private void RemoveSoundEntry(AudioSource audioSource)
    {
        if (activeSounds.ContainsKey(audioSource))
        {
            Destroy(activeSounds[audioSource]);
            activeSounds.Remove(audioSource);

            AdjustPanelSize();

            if (activeSounds.Count == 0)
            {
                visualAudio.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
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

    private void UpdateSoundIndicators(Vector3 soundPosition, Image leftIndicator, Image rightIndicator)
    {
        Transform playerCamera = Camera.main.transform;
        Vector3 directionToSound = soundPosition - playerCamera.position;
        float dotProduct = Vector3.Dot(playerCamera.right, directionToSound.normalized);

        if (dotProduct > 0)
        {
            rightIndicator.color = new Color(1, 1, 1, 1);
            leftIndicator.color = new Color(1, 1, 1, 0);
        }
        else
        {
            leftIndicator.color = new Color(1, 1, 1, 1);
            rightIndicator.color = new Color(1, 1, 1, 0);
        }
    }

    private void AdjustPanelSize()
    {
        int soundCount = activeSounds.Count;
        float newHeight = baseHeight + (entryHeight * soundCount);
        soundListContainer.sizeDelta = new Vector2(soundListContainer.sizeDelta.x, newHeight);
        visualAudio.sizeDelta = new Vector2(visualAudio.sizeDelta.x, newHeight);
    }

    // -------------------
    // PLAYBACK METHODS
    // -------------------

    public void PlaySfx(AudioClip clip, Transform spawnTransform, float volume, string soundName)
    {
        if (clip == null)
        {
            return;
        }

        // Create a temporary AudioSource
        GameObject tempAudioSource = new GameObject("NonPlayerAudioSource");
        tempAudioSource.transform.position = spawnTransform.position;

        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = soundFXMixerGroup; // Assign the SFX Mixer Group

        // Play the sound using PlayOneShot
        audioSource.PlayOneShot(clip);

        AddSoundEntry(audioSource, spawnTransform.position, soundName);

        // Destroy the AudioSource after the sound finishes playing
        Destroy(tempAudioSource, clip.length);
    }


    public void PlaySfxPlayer(AudioClip clip, Transform spawnTransform, float volume)
    {
        if (clip == null)
        {
            return;
        }

        // Create a temporary AudioSource
        GameObject tempAudioSource = new GameObject("PlayerAudioSource");
        tempAudioSource.transform.position = spawnTransform.position;

        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.transform.SetParent(spawnTransform); // Parent to the spawnTransform (player)
        audioSource.transform.localPosition = Vector3.zero;
        audioSource.spatialBlend = 1.0f;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = soundFXMixerGroup; // Assign the SFX Mixer Group

        // Play the sound using PlayOneShot
        audioSource.PlayOneShot(clip);

        // Destroy the AudioSource after the sound finishes playing
        Destroy(tempAudioSource, clip.length);
    }


    public void PlayRandomSfx(AudioClip[] clips, Transform spawnTransform, float volume, string soundName)
    {
        if (clips == null || clips.Length == 0)
        {
            return;
        }

        int rand = Random.Range(0, clips.Length);
        AudioClip selectedClip = clips[rand];

        // Create a temporary AudioSource
        GameObject tempAudioSource = new GameObject("TempAudioSource");
        tempAudioSource.transform.position = spawnTransform.position;

        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = selectedClip;
        audioSource.spatialBlend = 1.0f;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = soundFXMixerGroup; // Assign the SFX Mixer Group

        // Play the sound using PlayOneShot
        audioSource.PlayOneShot(selectedClip);

        AddSoundEntry(audioSource, spawnTransform.position, soundName);

        // Destroy the AudioSource after the sound finishes playing
        Destroy(tempAudioSource, selectedClip.length);
    }


    public void PlayRandomSfxPlayer(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        if (clips == null || clips.Length == 0)
        {
            return;
        }

        int rand = Random.Range(0, clips.Length);
        AudioClip selectedClip = clips[rand];

        // Create a temporary AudioSource
        GameObject tempAudioSource = new GameObject("TempAudioSource");
        tempAudioSource.transform.position = spawnTransform.position;

        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.transform.SetParent(spawnTransform); // Parent to the spawnTransform (player)
        audioSource.transform.localPosition = Vector3.zero;
        audioSource.clip = selectedClip;
        audioSource.spatialBlend = 1.0f;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = soundFXMixerGroup; // Assign the SFX Mixer Group

        // Play the sound using PlayOneShot
        audioSource.PlayOneShot(selectedClip);

        // Destroy the AudioSource after the sound finishes playing
        Destroy(tempAudioSource, selectedClip.length);
    }


    public void PlayLoopingSoundPersistent(Transform source, AudioClip clip, string soundName)
    {
        if (clip == null || source == null)
        {
            return;
        }

        AudioSource audioSource = source.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = source.gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }

        if (audioSource.clip != clip || !audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.volume = 0.2f;
            audioSource.spatialBlend = 1.0f;
            audioSource.Play();
        }
    }

    public void StopLoopingSoundPersistent(Transform source)
    {
        AudioSource audioSource = source.GetComponent<AudioSource>();
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
