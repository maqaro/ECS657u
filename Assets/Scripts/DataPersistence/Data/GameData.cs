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


    //Values in this contructor will be default values the game will start with when there's no data to load
    public GameData()
    {
        this.health = 100;
        this.kunaiCount = 100;
        this.spawnPoint = Vector3.zero;
        hasReachedCheckpoint = false;
        checkpointPosition = Vector3.zero;
    }
}
