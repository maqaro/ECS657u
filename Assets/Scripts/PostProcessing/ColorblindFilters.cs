using UnityEngine;
using UnityEngine.UI;

public class ColorblindFilters : MonoBehaviour
{
    public Toggle toggleNone;         // Toggle for Default (None)
    public Toggle toggleProtanopia;  // Toggle for Protanopia
    public Toggle toggleDeuteranopia;// Toggle for Deuteranopia
    public Toggle toggleTritanopia;  // Toggle for Tritanopia
    public Material colorBlindnessMaterial; // Material using the Color Blindness Shader

    private string currentFilterKey = "CurrentFilter"; // PlayerPrefs key for saving filter state

    void Start()
    {
        // Load the saved filter type (default to 0 - None)
        int savedFilter = PlayerPrefs.GetInt(currentFilterKey, 0);

        // Activate the saved filter
        ActivateFilter(savedFilter);

        // Add listeners to toggles to handle user selection dynamically
        toggleNone.onValueChanged.AddListener((isOn) => { if (isOn) SetFilter(0); });
        toggleProtanopia.onValueChanged.AddListener((isOn) => { if (isOn) SetFilter(1); });
        toggleDeuteranopia.onValueChanged.AddListener((isOn) => { if (isOn) SetFilter(2); });
        toggleTritanopia.onValueChanged.AddListener((isOn) => { if (isOn) SetFilter(3); });
    }

    private void SetFilter(int filterType)
    {
        // Save the selected filter type
        PlayerPrefs.SetInt(currentFilterKey, filterType);
        PlayerPrefs.Save();

        // Update the shader material property
        if (colorBlindnessMaterial != null)
        {
            colorBlindnessMaterial.SetFloat("_Type", filterType);
        }
    }

    private void ActivateFilter(int filterType)
    {
        // Activate the corresponding toggle
        toggleNone.isOn = (filterType == 0);
        toggleProtanopia.isOn = (filterType == 1);
        toggleDeuteranopia.isOn = (filterType == 2);
        toggleTritanopia.isOn = (filterType == 3);
    }
}
