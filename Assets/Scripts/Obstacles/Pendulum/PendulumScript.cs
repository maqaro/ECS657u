using UnityEngine;
using UnityEngine.Audio;

public class PendulumScript : MonoBehaviour
{
    [Header("Pendulum Settings")]
    public float speed = 1.5f; // Speed of pendulum swing
    public float limit = 40f; // Swing angle limit
    public bool randomStart = false; // Randomize start position
    private float random = 0;

    [Header("Knockback Settings")]
    public float knockbackForce = 20f; // Force applied to the player on collision

    [Header("Audio Settings")]
    [SerializeField] private AudioClip pendulumSound; // Pendulum sound effect
    [SerializeField] private float pendulumSoundVolume = 1.0f; // Volume of the pendulum sound
    [SerializeField] private Transform soundOrigin; // Sound's position relative to pendulum
    [SerializeField] private AudioMixerGroup soundFXMixerGroup; // Audio Mixer Group for SFX
    private AudioSource pendulumAudioSource; // AudioSource for pendulum sound

    private void Awake()
    {
        if (randomStart)
            random = Random.Range(0f, 1f);

        // Set the sound origin to the pendulum itself if not assigned
        if (soundOrigin == null)
        {
            Debug.LogWarning("Sound Origin not assigned. Defaulting to the transform of the pendulum.");
            soundOrigin = transform;
        }
    }

    private void Start()
    {
        // Initialize AudioSource dynamically
        pendulumAudioSource = gameObject.AddComponent<AudioSource>();
        pendulumAudioSource.clip = pendulumSound;
        pendulumAudioSource.volume = pendulumSoundVolume; // Set the volume
        pendulumAudioSource.loop = true; // Loop the pendulum sound
        pendulumAudioSource.spatialBlend = 1.0f; // Enable 3D sound
        pendulumAudioSource.minDistance = 1f; // Minimum distance for full volume
        pendulumAudioSource.maxDistance = 20f; // Maximum distance before sound fades
        pendulumAudioSource.dopplerLevel = 1.0f; // Enable Doppler effect

        // Assign the Audio Mixer Group to the AudioSource
        if (soundFXMixerGroup != null)
        {
            pendulumAudioSource.outputAudioMixerGroup = soundFXMixerGroup;
        }
        else
        {
            Debug.LogWarning("PendulumScript: No Audio Mixer Group assigned for sound effects.");
        }

        // Play the pendulum sound if a clip is assigned
        if (pendulumSound != null)
        {
            pendulumAudioSource.Play();
        }
    }

    private void Update()
    {
        // Swing the pendulum
        float angle = limit * Mathf.Sin(Time.time * speed + random);
        transform.localRotation = Quaternion.Euler(0, 90, angle);

        // Update sound position to follow the hammer's head
        if (pendulumAudioSource != null && soundOrigin != null)
        {
            pendulumAudioSource.transform.position = soundOrigin.position;
        }
    }

    // Detect collision with the player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Calculate knockback direction (from pendulum to player)
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                knockbackDirection.y = 0.5f; // Add upward force for knockback

                // Apply knockback force
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}
