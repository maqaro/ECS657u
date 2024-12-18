using UnityEngine;

public class PlayerRespawn : MonoBehaviour, IDataPersistence
{
    public float threshold;
    [SerializeField] private string spawnTag = "SpawnPoint";
    private Transform spawnPoint;
    private PlayerHealth playerHealth;
    public GameOverScreen gameOverScreen;
    private Rigidbody rb;

    void Start()
    {
        GameObject spawnObj = GameObject.FindGameObjectWithTag(spawnTag);
        if (spawnObj != null)
        {
            spawnPoint = spawnObj.transform;
        }
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody>();
    }

    public void LoadData(GameData data)
    {
        if (rb != null)
        {
            rb.position = data.spawnPoint;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            transform.position = data.spawnPoint;
        }
    }

    public void SaveData(ref GameData data)
    {
        data.spawnPoint = transform.position;
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
        if (other.CompareTag("Checkpoint"))
        {
            spawnPoint = other.transform;

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