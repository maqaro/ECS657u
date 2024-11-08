using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
    public static void LoadNextLevel()
    {
        // Find and disable the player object before loading
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
        }
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public static void LoadSpecificLevel(int levelIndex)
    {
        // Find and disable the player object before loading
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
        }
        
        SceneManager.LoadScene(levelIndex);
    }

    public static void ReloadLevel()
    {
        // Find and disable the player object before loading
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
        }
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
