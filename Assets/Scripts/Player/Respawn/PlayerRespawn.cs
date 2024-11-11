using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public float threshold;
    [SerializeField] private string respawnTag = "RespawnPoint";
    private Transform respawnPoint;
    private PlayerHealth playerHealth;
    public GameOverScreen gameOverScreen;

    void Start()
    {
        GameObject respawnObj = GameObject.FindGameObjectWithTag(respawnTag);
        if (respawnObj != null)
        {
            respawnPoint = respawnObj.transform;
        }
        playerHealth = GetComponent<PlayerHealth>();
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
        if (respawnPoint != null)
        {
            Vector3 adjustedRespawnPosition = respawnPoint.position;
            adjustedRespawnPosition.y += 1.0f;

            transform.position = adjustedRespawnPosition;

            if (playerHealth != null)
            {
                playerHealth.currentHealth = playerHealth.maxHealth;
                playerHealth.healthbar.SetHealth(playerHealth.currentHealth);
            }

            Rigidbody rb = GetComponent<Rigidbody>();
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
            respawnPoint = other.transform;

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
