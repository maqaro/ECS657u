using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class ExtendablePlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public GameObject extendablePlatform; // The platform to extend/retract
    public float speed = 1.0f; // Speed of movement
    public float distance; // Distance to extend/retract
    public Vector3 direction = Vector3.forward; // Direction of extension/retraction

    private Vector3 extendablePlatformStart;
    private Vector3 extendablePlatformEnd;
    public bool isOpen = false;
    private Coroutine extendablePlatformCoroutine;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip platformMoveSound; // Sound while platform is moving
    [SerializeField] private AudioMixerGroup soundFXMixerGroup; // Reference to the SFX mixer group

    private AudioSource platformAudioSource; // AudioSource for platform sounds

    private void Start()
    {
        if (extendablePlatform == null)
        {
            Debug.LogError("Extendable platform is not assigned.");
            return;
        }

        extendablePlatformStart = extendablePlatform.transform.position;
        extendablePlatformEnd = extendablePlatformStart + direction.normalized * distance;

        // Add a single AudioSource component for platform movement sound
        platformAudioSource = gameObject.AddComponent<AudioSource>();
        platformAudioSource.spatialBlend = 1.0f; // Enable 3D sound

        // Assign the mixer group to the AudioSource
        if (soundFXMixerGroup != null)
        {
            platformAudioSource.outputAudioMixerGroup = soundFXMixerGroup;
        }
    }

    public void ExtendPlatform()
    {
        if (!isOpen)
        {
            // Stop any existing coroutines
            StopPlatformAction();

            // Play platform movement sound
            PlayPlatformMovementSound();

            // Start platform extension coroutine
            extendablePlatformCoroutine = StartCoroutine(MovePlatform(extendablePlatform, extendablePlatformEnd));
            isOpen = true;
        }
    }

    public void RetractPlatform()
    {
        if (isOpen)
        {
            // Stop any existing coroutines
            StopPlatformAction();

            // Play platform movement sound
            PlayPlatformMovementSound();

            // Start platform retraction coroutine
            extendablePlatformCoroutine = StartCoroutine(MovePlatform(extendablePlatform, extendablePlatformStart));
            isOpen = false;
        }
    }

    public void TogglePlatform()
    {
        if (isOpen)
        {
            RetractPlatform();
        }
        else
        {
            ExtendPlatform();
        }
    }

    private IEnumerator MovePlatform(GameObject platform, Vector3 target)
    {
        while (Vector3.Distance(platform.transform.position, target) > 0.01f)
        {
            platform.transform.position = Vector3.MoveTowards(platform.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        platform.transform.position = target; // Ensure platform reaches the exact position

        // Stop movement sound
        StopPlatformMovementSound();
    }

    private void PlayPlatformMovementSound()
    {
        if (platformMoveSound != null)
        {
            // Use SoundFXManager to play the movement sound
            SoundFXManager.instance.PlaySfx(platformMoveSound, transform, 1.0f, "Sliding Wall");
        }
    }

    private void StopPlatformMovementSound()
    {
        if (platformAudioSource.isPlaying)
        {
            platformAudioSource.Stop();
        }
    }

    private void StopPlatformAction()
    {
        // Stop any ongoing coroutine
        if (extendablePlatformCoroutine != null)
        {
            StopCoroutine(extendablePlatformCoroutine);
        }

        // Stop the movement sound
        StopPlatformMovementSound();
    }
}
