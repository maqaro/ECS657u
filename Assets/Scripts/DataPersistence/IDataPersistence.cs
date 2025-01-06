using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence 
// Interface for data persistence
{
    void LoadData(GameData data);
    void SaveData(ref GameData data);
}
