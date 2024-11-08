using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Reset time scale to normal
        Time.timeScale = 1f;
        
        // Lock and hide cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        LevelLoader.LoadNextLevel();
    }

    public void QuitGame()
    {
        Debug.Log("QUIT FUNCTION WORKED");
        Application.Quit();
    }
}