using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // call to both player controls and InputAction from the new Unity Input system
    private Contols playerControls;
    private InputAction menu;

    //GameObject for the UI
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private bool isPaused;

    //initialise player controls
    void Awake()
    {
        playerControls = new Contols();
    }

    // when the menu is called
    private void OnEnable()
    {
        menu = playerControls.Menu.Escape;
        menu.Enable();
        menu.performed += Pause;
    }

    private void OnDisable()
    {
        menu.Disable();
        menu.performed -= Pause;
    }

    void Pause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        //checks if isPaused is true or false and determines whether to activate the menu or not
        if (isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }

    // this is what allows the menu to show up
    void ActivateMenu()
    {
        Time.timeScale = 0f; //completely freezes the game as opposed to slightly slowing it down

        //Removes all current sound
        AudioListener.pause = true;

        //UI will show up
        pauseUI.SetActive(true);

        // Show and unlock the cursor when the menu is active
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DeactivateMenu()
    {
        // This is essentially the opposite to what was implemented in ActivateMenu
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        pauseUI.SetActive(false);
        isPaused = false;

        // Hide and lock the cursor when the menu is deactivated
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadMainMenu()
    {
        // Resume time before changing scenes
        Time.timeScale = 1f;
        AudioListener.pause = false;

        //unlocks cursor once taken back to the mainMenu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // This is a call that takes you back to the MainMenu using SceneManager
        LevelLoader.LoadSpecificLevel(0);
    }
}
