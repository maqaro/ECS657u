using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;
    
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private IDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    public void Awake()
    {
        if (instance != null && instance != this)
        // If there is already an instance of the data persistence manager in the scene, destroy the newest one
        {
            Debug.Log("Found more than one data persistence manager instance in the scene. Destroying newest one");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Choose the appropriate data handler based on platform
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            this.dataHandler = new WebGLDataHandler(fileName);
        }
        else
        {
            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        }
    }

    private void OnEnable()
    {
        // Subscribe to scene loaded and unloaded events
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from scene loaded and unloaded events
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;  
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find all objects in the scene that implement the IDataPersistence interface
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Save game data when scene is unloaded
        SaveGame();
    }

    public void NewGame()
    {
        // Create a new instance of the game data
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load game data from file
        this.gameData = dataHandler.Load();

        // if no data was loaded, and we want to initialize data if null, start a new game
        if(this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        // if no data can be loaded, don't continue
        if (this.gameData == null)
        {
            return;
        }
        // push loaded data to all scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        //check if we have saved game data, if we don't log a warning
        if (gameData == null)
        {
            return;
        }

        // pass data to scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        // save data to file using data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        // Save game data when application is quit
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        // Check if game data is not null
        return gameData != null;
    }
}
