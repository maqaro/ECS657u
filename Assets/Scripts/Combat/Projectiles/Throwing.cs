using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Throwing : MonoBehaviour, IDataPersistence
{
    [SerializeField] private AudioClip[] throwSounds;

    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    public int totalThrows;
    public float throwCooldown = 1f; // Cooldown between throws
    public float throwForce = 20f; // Force applied when throwing
    public float throwUpwardForce = 2f;
    public bool showCooldownDebug = true;

    private float nextThrowTime = 0f;
    private bool readyToThrow = true;

    private Contols playerControls; // Reference to the player controls
    private InputAction throwAction;

    public GameObject pickUpHolder;

    // UI References
    public TextMeshProUGUI kunaiCounterText; // To display the number of throws
    public Image cooldownImage; // Grey overlay for the cooldown

    private DialogueUI dialogueUI; // Reference to DialogueUI

    private void Awake()
    {
        playerControls = new Contols(); // Initialize player controls
    }

    public void LoadData(GameData data)
    {
        this.totalThrows = data.kunaiCount;
        UpdateKunaiCounter(); // Update the UI when loading data
    }

    public void SaveData(ref GameData data)
    {
        data.kunaiCount = this.totalThrows;
    }

    private void OnEnable()
    {
        throwAction = playerControls.Player.Throw;
        throwAction.Enable();
        throwAction.performed += HandleThrow;

        // Find the DialogueUI in the scene
        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    private void OnDisable()
    {
        throwAction.Disable();
        throwAction.performed -= HandleThrow;
    }

    private void Start()
    {
        readyToThrow = true;

        // Initialize the kunai counter
        UpdateKunaiCounter();

        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // Start with no cooldown overlay
        }
    }

    private void Update()
    {
        // Update cooldown UI
        if (nextThrowTime > Time.time && cooldownImage != null)
        {
            float remainingTime = nextThrowTime - Time.time;
            cooldownImage.fillAmount = remainingTime / throwCooldown; // Adjust fill amount
        }
    }

    // Handles throw input and ensures throwing is not allowed during dialogue
    private void HandleThrow(InputAction.CallbackContext context)
    {
        // Prevent throwing if dialogue is active
        if (dialogueUI != null && dialogueUI.IsDialogueActive)
        {
            return;
        }

        if (Time.time < nextThrowTime)
        {
            return;
        }

        if (readyToThrow && totalThrows > 0 && pickUpHolder.transform.childCount == 0)
        {
            Throw();
            nextThrowTime = Time.time + throwCooldown;
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 1f; // Start cooldown fill
            }
        }
    }

    // Spawns a projectile and applies force
    private void Throw()
    {
        readyToThrow = false;

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        projectile.transform.Rotate(90f, 0f, 0f);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;
        SoundFXManager.instance.PlayRandomSfxPlayer(throwSounds, transform, 0.1f); // Adds audio for the kunai being thrown

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        Destroy(projectile, 5f);
        totalThrows--; // Reduces the kunai count

        // Update the UI text
        UpdateKunaiCounter();

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    // Resets throwing state and updates UI
    private void ResetThrow()
    {
        readyToThrow = true;
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // Reset cooldown overlay
        }
    }

    // Adds throws to the total counter
    public void AddThrows(int amount)
    {
        totalThrows += amount;

        // Update the UI text when adding throws
        UpdateKunaiCounter();
    }

    // Updates the UI text for kunai count
    private void UpdateKunaiCounter()
    {
        if (kunaiCounterText != null)
        {
            kunaiCounterText.text = $"{totalThrows}";
        }
    }
}
