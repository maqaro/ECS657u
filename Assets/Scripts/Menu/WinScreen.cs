using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public GameObject winScreenUI;

    private void Start()
    {
        winScreenUI.SetActive(false);
        // Make sure cursor is hidden and locked at start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WinTrigger"))
        Debug.Log("Entered Trigger: " + other.tag);
        if (other.CompareTag("Player")) // Change to match your player tag if needed
        {
            Debug.Log("Win Trigger Activated");
            Setup();
        }
    }

    public void Setup()
    {
        winScreenUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        // Show and unlock cursor
        Debug.Log("Setting winScreenUI to active");
        winScreenUI.SetActive(true);  // Show the UI
        Time.timeScale = 1f;  // Keep game running to ensure UI is visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MainMenuButton()
    {
        // Resume time before changing scenes
        Time.timeScale = 1f;
        AudioListener.pause = false;

        //unlocks cursor for the main menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
    }
}