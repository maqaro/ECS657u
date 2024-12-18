using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float health;
    public int kunaiCount;

    public GameData()
    {
        this.health = 100;
        this.kunaiCount = 100;
    }
}
