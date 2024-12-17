using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // What the game will save
    public int currentLevel;
    public int currentHealth;
    public Vector3 currentPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Save()
    {
        currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        string savePath = Application.persistentDataPath + "/playerInfo.dat";
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Create(savePath))
        {
            PlayerData_Storage data = new PlayerData_Storage()
            {
                currentLevel = currentLevel,
                currentHealth = currentHealth,
                position = new SerializableVector3(currentPosition)
            };
            bf.Serialize(file, data);
        }
    }

    public void Load()
    {
        string savePath = Application.persistentDataPath + "/playerInfo.dat";
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(savePath, FileMode.Open))
            {
                PlayerData_Storage data = (PlayerData_Storage)bf.Deserialize(file);
                currentLevel = data.currentLevel;
                currentHealth = data.currentHealth;
                currentPosition = data.position.ToVector3();
            }
        }
    }
}

[Serializable]
class PlayerData_Storage
{
    public int currentLevel;
    public int currentHealth;
    public SerializableVector3 position;
}

[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}