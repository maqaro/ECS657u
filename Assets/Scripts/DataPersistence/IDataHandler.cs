using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataHandler
// Interface for data handlers
{
    GameData Load();
    void Save(GameData data);
}
