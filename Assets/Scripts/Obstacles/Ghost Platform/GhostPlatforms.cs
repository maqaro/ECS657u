using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class GhostPlatforms : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float disappearTime = 3;

    [Header("Reset Settings")]
    [SerializeField] private bool canReset = false;
    [SerializeField] private float resetTime;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip crumbleSound; // Sound to play when platform crumbles
    [SerializeField] private Transform soundOrigin; // Origin for the sound (optional, defaults to platform)

    private Animator myAnim;

    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
        myAnim.SetFloat("DisappearTime", 1 / disappearTime);

        // If no specific sound origin is assigned, use the platform's transform
        if (soundOrigin == null)
        {
            soundOrigin = transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag(playerTag))
        {
            myAnim.SetBool("Trigger", true);

            // Schedule the crumble sound to play when the platform finishes disappearing
            Invoke(nameof(PlayCrumbleSound), disappearTime);
        }
    }

    private void PlayCrumbleSound()
    {
        if (crumbleSound != null)
        {
            // Use SoundFXManager to play the crumble sound
            SoundFXManager.instance.PlaySfx(crumbleSound, soundOrigin, 1.0f, "Crumble");
        }
    }

    public void TriggerReset()
    {
        if (canReset)
        {
            StartCoroutine(Reset());
        }
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(resetTime);
        myAnim.SetBool("Trigger", false);
    }
}
