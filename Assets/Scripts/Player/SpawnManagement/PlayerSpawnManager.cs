using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour, IDataPersistence
{
    public float threshold;
    [SerializeField] private string spawnTag = "SpawnPoint";
    private Transform spawnPoint;
    private PlayerHealth playerHealth;
    public GameOverScreen gameOverScreen;
    private Rigidbody rb;
    private Vector3 lastCheckpointPosition;
    private bool hasReachedCheckpoint = false;

    void Start()
    {
        GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnTag);
        if (spawnObj != null)
        {
            spawnPoint = spawnObj.transform;
            lastCheckpointPosition = spawnPoint.position;
        }
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody>();
    }

    public void LoadData(GameData data)
    {
        if (rb != null)
        {
            hasReachedCheckpoint = data.hasReachedCheckpoint;
            Vector3 loadPosition = hasReachedCheckpoint ? data.checkpointPosition : spawnPoint.position;
            rb.position = loadPosition;
            lastCheckpointPosition = loadPosition;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            transform.position = hasReachedCheckpoint ? data.checkpointPosition : data.spawnPoint;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.hasReachedCheckpoint = hasReachedCheckpoint;
        data.checkpointPosition = lastCheckpointPosition;
        data.spawnPoint = spawnPoint != null ? spawnPoint.position : Vector3.zero;
    }

    void Update()
    {
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
        if (other.CompareTag("SpawnPoint"))
        {
            hasReachedCheckpoint = true;
            spawnPoint = other.transform;
            lastCheckpointPosition = spawnPoint.position;
            Debug.Log("Reached spawn point");
            
        }
        else if (other.CompareTag("Checkpoint"))
        {
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