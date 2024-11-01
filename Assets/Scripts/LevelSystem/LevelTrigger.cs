using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindAnyObjectByType<LevelLoader>().LoadNextLevel();
        }
    }
}
