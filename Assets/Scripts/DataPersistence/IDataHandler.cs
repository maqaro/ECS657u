using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataHandler
{
    GameData Load();
    void Save(GameData data);
}
