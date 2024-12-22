using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text SensitivityTextValue = null;
    [SerializeField] private Slider SensSlider = null;
    [SerializeField] private float defaultSen = 1.0f; // Default sensitivity as a float
    public float mainSensitivity = 1.0f; // Changed to float

    public void OnNewGameClicked()
    {
        // Reset time scale to normal
        Time.timeScale = 1f;

        // Lock and hide cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LevelLoader.LoadNextLevel();
        DataPersistenceManager.instance.NewGame();
    }

    public void OnLoadGameClicked()
    {
        DataPersistenceManager.instance.LoadGame();
    }

    public void OnSaveGameClicked()
    {
        DataPersistenceManager.instance.SaveGame();
    }

    public void QuitGame()
    {
        Debug.Log("QUIT FUNCTION WORKED");
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        //StartCoroutine(ConfirmationBox());
        Debug.Log($"Volume Changed to: {AudioListener.volume.ToString("0.0")}");
    }

    public void SetSensitivity(float sensitivity)
    {
        mainSensitivity = sensitivity; // No need to round since it's now a float
        SensitivityTextValue.text = sensitivity.ToString("0.0"); // Display as a float
        Debug.Log($"Sensitivity set to: {mainSensitivity}");
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSen", mainSensitivity); // Save as a float
        //StartCoroutine(ConfirmationBox());
    }

    //Resets the value of the Volume
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            SensitivityTextValue.text = defaultSen.ToString("0.0"); // Display default sensitivity as a float
            SensSlider.value = defaultSen;
            mainSensitivity = defaultSen;
            GameplayApply();
        }
    }

    //This is purely just to confirm the volume changing works especially as we have no sound currently
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
