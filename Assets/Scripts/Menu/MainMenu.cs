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

    public void LoadGame()
    {
        Time.timeScale = 1f;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        SaveManager.Instance.Load();
        LevelLoader.LoadSpecificLevel(SaveManager.Instance.currentLevel);
    }

    public void SaveGame()
    {
        SaveManager.Instance.Save();
    }

    public void QuitGame()
    {
        Debug.Log("QUIT FUNCTION WORKED");
        Application.Quit();
    }
}