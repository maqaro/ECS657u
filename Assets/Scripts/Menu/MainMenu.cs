using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text masterVolumeText = null;
    [SerializeField] private TMP_Text soundFXVolumeText = null;
    [SerializeField] private TMP_Text bgVolumeText = null;
    [SerializeField] private Slider masterVolumeSlider = null;
    [SerializeField] private Slider soundFXVolumeSlider = null;
    [SerializeField] private Slider bgVolumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text SensitivityTextValue = null;
    [SerializeField] private Slider SensSlider = null;
    [SerializeField] private float defaultSen = 1.0f; // Default sensitivity as a float
    public float mainSensitivity = 1.0f; // Changed to float

    [Header("Sound Manager")]
    [SerializeField] private SoundMixerManager soundMixerManager; // Reference to SoundMixerManager

    public void Start()
    {
        // Disable continue button if no saved data is available
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
        }

        // Load saved volume and sensitivity values or use defaults
        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
        soundFXVolumeSlider.value = PlayerPrefs.GetFloat("soundFXVolume", defaultVolume);
        bgVolumeSlider.value = PlayerPrefs.GetFloat("bgVolume", defaultVolume);
        SensSlider.value = PlayerPrefs.GetFloat("masterSen", defaultSen);

        // Initialise the settings
        OnMasterVolumeChanged(masterVolumeSlider.value);
        OnSoundFXVolumeChanged(soundFXVolumeSlider.value);
        OnBGVolumeChanged(bgVolumeSlider.value);
        SetSensitivity(SensSlider.value);
    }

    public void OnNewGameClicked()
    {
        DisableMenuButtons();
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void OnContinueGameClicked()
    {
        DisableMenuButtons();
        DataPersistenceManager.instance.LoadGame();
        SceneManager.LoadSceneAsync("SampleScene");
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

    // Adjusts the master volume
    public void OnMasterVolumeChanged(float volume)
    {
        soundMixerManager.SetMasterVolume(volume); // Update mixer
        masterVolumeText.text = volume.ToString("0.0"); // Update UI
        PlayerPrefs.SetFloat("masterVolume", volume); // Save to PlayerPrefs
    }

    // Adjusts the sound effects volume
    public void OnSoundFXVolumeChanged(float volume)
    {
        soundMixerManager.SetSoundFXVolume(volume); // Update mixer
        soundFXVolumeText.text = volume.ToString("0.0"); // Update UI
        PlayerPrefs.SetFloat("soundFXVolume", volume); // Save to PlayerPrefs
    }

    // Adjusts the background volume
    public void OnBGVolumeChanged(float volume)
    {
        soundMixerManager.SetBGVolume(volume); // Update mixer
        bgVolumeText.text = volume.ToString("0.0"); // Update UI
        PlayerPrefs.SetFloat("bgVolume", volume); // Save to PlayerPrefs
    }

    // Adjusts sensitivity settings
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

    // Resets the value of the volume and/or sensitivity
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            masterVolumeSlider.value = defaultVolume;
            soundFXVolumeSlider.value = defaultVolume;
            bgVolumeSlider.value = defaultVolume;

            OnMasterVolumeChanged(defaultVolume);
            OnSoundFXVolumeChanged(defaultVolume);
            OnBGVolumeChanged(defaultVolume);
        }

        if (MenuType == "Gameplay")
        {
            SensitivityTextValue.text = defaultSen.ToString("0.0"); // Display default sensitivity as a float
            SensSlider.value = defaultSen;
            mainSensitivity = defaultSen;
            GameplayApply();
        }
    }

    public void DisableMenuButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }

    // This is purely just to confirm the volume changing works, especially as we have no sound currently
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
