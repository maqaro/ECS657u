using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MovingDoor : MonoBehaviour
{
    [Header("Door References")]
    public GameObject leftDoor;
    public GameObject rightDoor;

    [Header("Door Settings")]
    public float speed = 1.0f; // Speed of door movement
    public bool isOpen = false; // Door state (open or closed)

    [Header("Audio Settings")]
    [SerializeField] private AudioClip doorMoveSound; // Sound while door is moving
    [SerializeField] private AudioClip doorOpenCloseSound; // Sound when door finishes opening/closing
    [SerializeField] private AudioMixerGroup soundFXMixerGroup; // Reference to the SFX mixer group

    private Vector3 leftDoorStart;
    private Vector3 rightDoorStart;
    private Vector3 leftDoorEnd;
    private Vector3 rightDoorEnd;

    private Coroutine doorCoroutine; // Single coroutine for door actions
    private AudioSource doorAudioSource; // Single AudioSource for the door

    private void Start()
    {
        if (leftDoor == null || rightDoor == null)
        {
            Debug.LogError("Left or Right door is not assigned.");
            return;
        }

        // Initialize door positions
        leftDoorStart = leftDoor.transform.position;
        rightDoorStart = rightDoor.transform.position;
        leftDoorEnd = leftDoorStart + Vector3.forward * 2.0f;
        rightDoorEnd = rightDoorStart + Vector3.back * 2.0f;

        // Add a single AudioSource component for the door
        doorAudioSource = gameObject.AddComponent<AudioSource>();
        doorAudioSource.spatialBlend = 1.0f; // 3D sound

        // Assign the mixer group to the AudioSource
        if (soundFXMixerGroup != null)
        {
            doorAudioSource.outputAudioMixerGroup = soundFXMixerGroup;
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            // Stop any ongoing actions
            StopDoorAction();

            // Play door movement sound
            PlayDoorMovementSound();

            // Start door opening coroutine
            doorCoroutine = StartCoroutine(MoveDoors(leftDoorEnd, rightDoorEnd));
            isOpen = true;
        }
    }

    public void CloseDoor()
    {
        if (isOpen)
        {
            // Stop any ongoing actions
            StopDoorAction();

            // Play door movement sound
            PlayDoorMovementSound();

            // Start door closing coroutine
            doorCoroutine = StartCoroutine(MoveDoors(leftDoorStart, rightDoorStart));
            isOpen = false;
        }
    }

    private IEnumerator MoveDoors(Vector3 leftTarget, Vector3 rightTarget)
    {
        while (Vector3.Distance(leftDoor.transform.position, leftTarget) > 0.01f ||
               Vector3.Distance(rightDoor.transform.position, rightTarget) > 0.01f)
        {
            // Move the doors towards their target positions
            leftDoor.transform.position = Vector3.MoveTowards(leftDoor.transform.position, leftTarget, speed * Time.deltaTime);
            rightDoor.transform.position = Vector3.MoveTowards(rightDoor.transform.position, rightTarget, speed * Time.deltaTime);

            yield return null;
        }

        // Ensure doors reach their exact positions
        leftDoor.transform.position = leftTarget;
        rightDoor.transform.position = rightTarget;

        // Stop movement sound and play open/close sound
        StopDoorMovementSound();
        SoundFXManager.instance.PlaySfx(doorOpenCloseSound, transform, 0.4f, "Door Lock");
    }

    private void PlayDoorMovementSound()
    {
        if (doorMoveSound != null)
        {
            // Restart the movement sound
            doorAudioSource.Stop();
            doorAudioSource.clip = doorMoveSound;
            doorAudioSource.loop = false; // Ensure it doesn't loop
            doorAudioSource.Play();
        }
    }

    private void StopDoorMovementSound()
    {
        if (doorAudioSource.isPlaying)
        {
            doorAudioSource.Stop();
        }
    }

    private void StopDoorAction()
    {
        // Stop any ongoing coroutine
        if (doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }

        // Stop the movement sound
        StopDoorMovementSound();
    }
}
