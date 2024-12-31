using UnityEngine;

public class SoundMonitor : MonoBehaviour
{
    public AudioSource audioSource; // Attach the test dummy's AudioSource here
    public string soundName = "Rain"; // Name of the sound
    public Transform player; // Player object (drag the player or camera here)

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            SoundFXManager.instance.MonitorSound(audioSource, transform, soundName);
        }
    }
}
