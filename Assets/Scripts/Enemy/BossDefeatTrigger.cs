using UnityEngine;

public class BossDefeatTrigger : MonoBehaviour
{
    public GameObject winScreenUI;  // Reference to the Win Menu UI

    private void Start()
    {
        // Ensure the Win Screen UI is hidden at the start
        if (winScreenUI != null)
        {
            winScreenUI.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Boss defeated - showing win screen");

        // Stop the game
        Time.timeScale = 0f;

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Activate the Win Screen UI
        if (winScreenUI != null)
        {
            winScreenUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Win Screen UI is not assigned!");
        }
    }
}
