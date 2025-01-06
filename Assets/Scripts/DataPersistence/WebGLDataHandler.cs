using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLDataHandler : IDataHandler
{
    private string saveKey = "";

    public WebGLDataHandler(string key)
    // Constructor that takes a string key as an argument
    {
        // Set the save key to the key passed in
        this.saveKey = key;
    }

    public GameData Load()
    // Load method that returns a GameData object
    {
        GameData loadedData = null;
        // Check if the key exists in the PlayerPrefs
        if (PlayerPrefs.HasKey(saveKey))
        {
            // Load the JSON data from the PlayerPrefs
            string jsonData = PlayerPrefs.GetString(saveKey);
            // Deserialize the JSON data into a GameData object
            loadedData = JsonUtility.FromJson<GameData>(jsonData);
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        // Serialize the GameData object into JSON
        string jsonData = JsonUtility.ToJson(data, true);
        // Save the JSON data to the PlayerPrefs
        PlayerPrefs.SetString(saveKey, jsonData);
        PlayerPrefs.Save();
    }
}
