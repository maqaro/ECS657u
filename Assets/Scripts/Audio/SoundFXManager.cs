using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [Header("UI Elements")]
    [SerializeField] private RectTransform visualAudio;
    [SerializeField] private RectTransform soundListContainer;
    [SerializeField] private GameObject soundEntryPrefab;

    [Header("Player Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;

    [Header("Audio Source Pool Settings")]
    [SerializeField] private int poolSize = 10;
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();

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

        // Initialize the audio source pool
        InitializeAudioSourcePool();
        visualAudio.gameObject.SetActive(false);
    }

    private void InitializeAudioSourcePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource pooledSource = new GameObject("PooledAudioSource").AddComponent<AudioSource>();
            pooledSource.gameObject.SetActive(false);
            audioSourcePool.Enqueue(pooledSource);
        }
    }

    private AudioSource GetPooledAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            AudioSource source = audioSourcePool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }
        else
        {
            Debug.LogWarning("AudioSource pool is empty. Consider increasing the pool size.");
            return new GameObject("TempAudioSource").AddComponent<AudioSource>();
        }
    }

    private void ReturnToPool(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        audioSourcePool.Enqueue(source);
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
            Debug.LogError("AddSoundEntry: Missing components in Sound Entry Prefab. Please check the prefab setup.");
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

    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume, string soundName)
    {
        AudioSource audioSource = GetPooledAudioSource();
        audioSource.transform.position = spawnTransform.position;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f;
        audioSource.Play();

        AddSoundEntry(audioSource, spawnTransform.position, soundName);
        StartCoroutine(ReturnToPoolAfterPlayback(audioSource, clip.length));
    }

    public void PlaySoundFXClipPlayer(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = GetPooledAudioSource();
        audioSource.transform.position = spawnTransform.position;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f;
        audioSource.Play();

        StartCoroutine(ReturnToPoolAfterPlayback(audioSource, clip.length));
    }

    public void PlayRandomSoundFXClip(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("PlayRandomSoundFXClip: No audio clips provided!");
            return;
        }

        int rand = Random.Range(0, clips.Length);
        AudioClip selectedClip = clips[rand];

        AudioSource audioSource = GetPooledAudioSource();
        audioSource.transform.position = spawnTransform.position;
        audioSource.clip = selectedClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f;
        audioSource.Play();

        StartCoroutine(ReturnToPoolAfterPlayback(audioSource, selectedClip.length));
    }

    public void PlayRandomSoundFXClipPlayer(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("PlayRandomSoundFXClipPlayer: No audio clips provided!");
            return;
        }

        int rand = Random.Range(0, clips.Length);
        AudioClip selectedClip = clips[rand];

        AudioSource audioSource = GetPooledAudioSource();
        audioSource.transform.position = spawnTransform.position;
        audioSource.clip = selectedClip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1.0f; // Ensure spatial audio
        audioSource.Play();

        // Return the AudioSource to the pool after playback
        StartCoroutine(ReturnToPoolAfterPlayback(audioSource, selectedClip.length));
    }

    private IEnumerator ReturnToPoolAfterPlayback(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (audioSource != null)
        {
            ReturnToPool(audioSource);
        }
    }

    public void PlayLoopingSoundPersistent(Transform source, AudioClip clip, string soundName)
    {
        if (clip == null || source == null)
        {
            Debug.LogWarning("PlayLoopingSoundPersistent: Invalid clip or source!");
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
            audioSource.volume = 0.3f;
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
