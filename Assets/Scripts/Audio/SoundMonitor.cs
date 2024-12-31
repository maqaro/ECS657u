using UnityEngine;

public class SoundMonitor : MonoBehaviour
{
    public AudioSource audioSource; // Attach the AudioSource on the sound source
    public Transform player; // Player object (drag the player or camera here)

    private void Update()
    {
        // Skip monitoring if the sound source is tagged as "Player"
        if (gameObject.CompareTag("Player"))
        {
            return;
        }

        if (audioSource.isPlaying)
        {
            string soundName = audioSource.clip != null ? audioSource.clip.name : "Unknown"; // Get the name of the AudioClip
            SoundFXManager.instance.MonitorSound(audioSource, transform, soundName);
        }
    }
}
