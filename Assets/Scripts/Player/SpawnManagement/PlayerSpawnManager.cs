using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour, IDataPersistence
{
    public float threshold;
    [SerializeField] private string spawnTag = "SpawnPoint";
    [SerializeField] private AudioClip checkpointSound;
    private Transform spawnPoint;
    private PlayerHealth playerHealth;
    public GameOverScreen gameOverScreen;
    private Rigidbody rb;
    private Vector3 lastCheckpointPosition;
    private bool hasReachedCheckpoint = false;
    private bool isInitialized = false;
    

    void Start()
    {
        InitializeIfNeeded();
    }

    private void InitializeIfNeeded()
    {
        if (!isInitialized)
        {
            GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnTag);
            if (spawnObj != null)
            {
                spawnPoint = spawnObj.transform;
                // Only set lastCheckpointPosition if we haven't loaded data yet
                if (!hasReachedCheckpoint)
                {
                    lastCheckpointPosition = spawnPoint.position;
                }
            }
            playerHealth = GetComponent<PlayerHealth>();
            rb = GetComponent<Rigidbody>();
            isInitialized = true;
        }
    }

    public void LoadData(GameData data)
    {
        InitializeIfNeeded();
        
        this.hasReachedCheckpoint = data.hasReachedCheckpoint;
        this.lastCheckpointPosition = data.checkpointPosition;

        Vector3 loadPosition = hasReachedCheckpoint ? lastCheckpointPosition : data.spawnPoint;
        
        if (rb != null)
        {
            rb.position = loadPosition;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            transform.position = hasReachedCheckpoint ? lastCheckpointPosition : data.spawnPoint;
        }
    }

    public void SaveData(ref GameData data)
    {
        // Saving the players position

        // Saving the checkpoint position
        data.hasReachedCheckpoint = hasReachedCheckpoint;
        data.checkpointPosition = lastCheckpointPosition;

        // Saving the spawn point
        data.spawnPoint = spawnPoint != null ? spawnPoint.position : Vector3.zero;
    }

    void Update()
    {
        // Check if the player is dead or if the player fell below the map
        if ((playerHealth != null && playerHealth.currentHealth <= 0) || transform.position.y < threshold)
        {
            gameOverScreen.Setup();
        }
    }

    public void RespawnPlayer()
    {
        if (spawnPoint != null)
        {
            Vector3 adjustedRespawnPosition = spawnPoint.position;
            adjustedRespawnPosition.y += 1.0f;

            transform.position = adjustedRespawnPosition;

            if (playerHealth != null)
            {
                playerHealth.currentHealth = playerHealth.maxHealth;
                playerHealth.healthbar.SetHealth(playerHealth.currentHealth);
            }


            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint") || other.CompareTag("SpawnPoint"))
        {
            SoundFXManager.instance.PlaySfx(checkpointSound, transform, 2f, "Checkpoint");
            hasReachedCheckpoint = true;
            spawnPoint = other.transform;
            lastCheckpointPosition = spawnPoint.position;

            Renderer checkpointRenderer = other.GetComponent<Renderer>();
            Collider checkpointCollider = other.GetComponent<Collider>();

            if (checkpointRenderer != null)
            {
                checkpointRenderer.enabled = false;
            }

            if (checkpointCollider != null)
            {
                checkpointCollider.enabled = false;
            }
        }
    }
}