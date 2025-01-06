using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject gameOverUI;
    private PlayerSpawnManager playerRespawn;

    private void Start()
    {
        playerRespawn = Object.FindFirstObjectByType<PlayerSpawnManager>();
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
        // Save the game before loading up the main menu
        DataPersistenceManager.instance.SaveGame();

        // Resume time before changing scenes
        Time.timeScale = 1f;
        AudioListener.pause = false;

        //unlocks cursor once taken back to the mainMenu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // This is a call that takes you back to the MainMenu using SceneManager
        SceneManager.LoadScene("MainMenu");
    }
}
