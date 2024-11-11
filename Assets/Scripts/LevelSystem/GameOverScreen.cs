using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject gameOverUI;
    private PlayerRespawn playerRespawn;

    private void Start()
    {
        playerRespawn = Object.FindFirstObjectByType<PlayerRespawn>();
        gameOverUI.SetActive(false);
        // Make sure cursor is hidden and locked at start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Setup()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        // Show and unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        gameOverUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        // Hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RespawnButton()
    {
        Hide();
        if (playerRespawn != null)
        {
            playerRespawn.RespawnPlayer();
        }
    }

    public void MainMenuButton()
    {
        // Don't call Hide() since it locks the cursor
        Time.timeScale = 1f; // Resume normal time scale
        
        // Make cursor visible and unlocked for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        LevelLoader.LoadSpecificLevel(0);
    }
}
