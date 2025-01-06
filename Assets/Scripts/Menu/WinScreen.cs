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
        {
            Setup();
        }
    }

    public void Setup()
    {
        winScreenUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        // Show and unlock cursor
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
