using UnityEngine;

public class SoundMonitor : MonoBehaviour
{
    public AudioSource audioSource; // Attach the AudioSource on the sound source
    public string soundName = "Placeholder"; // Name of the sound
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
            SoundFXManager.instance.MonitorSound(audioSource, transform, soundName);
        }
    }
}
