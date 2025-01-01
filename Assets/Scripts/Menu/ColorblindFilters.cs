using UnityEngine;
using UnityEngine.UI;

public class ColorblindFilters : MonoBehaviour
{
    public Toggle toggleNone;
    public Toggle toggleProtanopia;
    public Toggle toggleDeuteranopia;
    
    void Start()
    {
        if (PlayerPrefs.GetInt("ToggleBool") == 1)
        {
            toggleNone.isOn = true;
        }
        else {
            toggleNone.isOn = false;
        }

        if (PlayerPrefs.GetInt("ToggleBool") == 1)
        {
            toggleProtanopia.isOn = true;
        }
        else {
            toggleProtanopia.isOn = false;
        }


        if (PlayerPrefs.GetInt("ToggleBool") == 1)
        {
            toggleDeuteranopia.isOn = true;
        }
        else {
            toggleDeuteranopia.isOn = false;
        }

    }

    
    void Update()
    {
        if (toggleNone.isOn == true)
        {
            PlayerPrefs.SetInt("ToggleBool", 1);
        }
        else {
            PlayerPrefs.SetInt("ToggleBool", 0);
        }

        if (toggleProtanopia.isOn == true)
        {
            PlayerPrefs.SetInt("ToggleBool", 1);
        }
        else {
            PlayerPrefs.SetInt("ToggleBool", 0);
        }

        if (toggleDeuteranopia.isOn == true)
        {
            PlayerPrefs.SetInt("ToggleBool", 1);
        }
        else {
            PlayerPrefs.SetInt("ToggleBool", 0);
        }
        
    }
}
