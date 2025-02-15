using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float health;
    public int kunaiCount;
    public Vector3 spawnPoint;
    public bool hasReachedCheckpoint;
    public Vector3 checkpointPosition;
    public SerializableDictionary<string, bool> kunaiPickupsCollected;
    public SerializableDictionary<string, bool> healthPickupsCollected;
    public SerializableDictionary<string, bool> powerUpsCollected;


    //Values in this contructor will be default values the game will start with when there's no data to load
    public GameData()
    {
        this.health = 100;
        this.kunaiCount = 25;
        this.spawnPoint = Vector3.zero;
        hasReachedCheckpoint = false;
        checkpointPosition = Vector3.zero;
        kunaiPickupsCollected = new SerializableDictionary<string, bool>();
        healthPickupsCollected = new SerializableDictionary<string, bool>();
        powerUpsCollected = new SerializableDictionary<string, bool>();
    }
}
