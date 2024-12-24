using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Found more than one data persistence manager instance in the scene.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // TODO:
        // Load game data from file
        this.gameData = dataHandler.Load();

        // if no data can be loaded initialize a new game
        if (this.gameData == null)
        {
            Debug.Log("No game data found. Starting new game.");
            NewGame();
        }
        // push loaded data to all scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded player Health: " + gameData.health);
        Debug.Log("Loaded player Kunai Count: " + gameData.kunaiCount);
        Debug.Log("Loaded player Position: " + gameData.spawnPoint);
        Debug.Log("Loaded player kunaiPickupsCollected: " + gameData.kunaiPickupsCollected.Count);
        Debug.Log("Loaded player healthPickupsCollected: " + gameData.healthPickupsCollected.Count);
    }

    public void SaveGame()
    {
        // pass data to scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        Debug.Log("Saved player Health: " + gameData.health);
        Debug.Log("Saved player Kunai Count: " + gameData.kunaiCount);
        Debug.Log("Saved player Position: " + gameData.spawnPoint);
        Debug.Log("Saved player kunaiPickupsCollected: " + gameData.kunaiPickupsCollected.Count);
        Debug.Log("Saved player healthPickupsCollected: " + gameData.healthPickupsCollected.Count);

        // save data to file using data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
