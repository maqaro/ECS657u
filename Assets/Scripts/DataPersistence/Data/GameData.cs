using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float health;
    public int kunaiCount;
    public Vector3 spawnPoint;


    //Values in this contructor will be default values the game will start with when there's no data to load
    public GameData()
    {
        this.health = 100;
        this.kunaiCount = 100;
        this.spawnPoint = new Vector3(-10.51f, 1f, 0);
    }
}
