using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject; // Prefab for audio playback
    [SerializeField] private AudioDetector audioDetector; // Reference to the AudioDetector script

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        // Play audio
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();

        // Call the AudioDetector to visualize the sound
        if (audioDetector != null)
        {
            audioDetector.DetectAndVisualizeSound(spawnTransform, clip.name);
        }

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, clips.Length);

        // Play audio
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clips[rand];
        audioSource.volume = volume;
        audioSource.Play();

        // Call the AudioDetector to visualize the sound
        if (audioDetector != null)
        {
            audioDetector.DetectAndVisualizeSound(spawnTransform, clips[rand].name);
        }

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
