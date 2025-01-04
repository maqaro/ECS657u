using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // For the new Input System
using UnityEngine.UI; // For UI Text
using TMPro; // If using TextMeshPro

public class Throwing : MonoBehaviour, IDataPersistence
{
    [SerializeField] private AudioClip[] throwSounds;

    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    public int totalThrows;
    public float throwCooldown = 1f;
    public float throwForce = 20f;
    public float throwUpwardForce = 2f;
    public bool showCooldownDebug = true;

    private float nextThrowTime = 0f;
    private bool readyToThrow = true;

    private Contols playerControls;
    private InputAction throwAction;

    public GameObject pickUpHolder;

    // UI References
    public TextMeshProUGUI kunaiCounterText; // To display the number of throws
    public Image cooldownImage; // Grey overlay for the cooldown

    private void Awake()
    {
        playerControls = new Contols();
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

    private void HandleThrow(InputAction.CallbackContext context)
    {
        if (Time.time < nextThrowTime)
        {
            if (showCooldownDebug)
            {
                Debug.Log($"Throw on cooldown. Ready in {nextThrowTime - Time.time:F1} seconds");
            }
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

    private void Throw()
    {
        readyToThrow = false;

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        projectile.transform.Rotate(90f, 0f, 0f);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceDirection = cam.transform.forward;
        SoundFXManager.instance.PlayRandomSoundFXClipPlayer(throwSounds, transform, 0.1f);

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce + Vector3.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        Destroy(projectile, 5f);
        totalThrows--;

        // Update the UI text
        UpdateKunaiCounter();

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // Reset cooldown overlay
        }
    }

    public void AddThrows(int amount)
    {
        totalThrows += amount;

        // Update the UI text when adding throws
        UpdateKunaiCounter();
    }

    // Method to update the UI text
    private void UpdateKunaiCounter()
    {
        if (kunaiCounterText != null)
        {
            kunaiCounterText.text = $"{totalThrows}";
        }
    }
}
