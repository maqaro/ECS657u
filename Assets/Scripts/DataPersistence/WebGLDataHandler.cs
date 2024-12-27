using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLDataHandler : IDataHandler
{
    private string saveKey = "";

    public WebGLDataHandler(string key)
    {
        this.saveKey = key;
    }

    public GameData Load()
    {
        GameData loadedData = null;
        if (PlayerPrefs.HasKey(saveKey))
        {
            string jsonData = PlayerPrefs.GetString(saveKey);
            loadedData = JsonUtility.FromJson<GameData>(jsonData);
        }
        return loadedData;
    }

    public void Save(GameData data)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(saveKey, jsonData);
        PlayerPrefs.Save();
    }
}
