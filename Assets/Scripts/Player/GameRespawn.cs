using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    public float threshold;
    [SerializeField] string enemyTag = "enemy";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(enemyTag))
        {
            other.gameObject.transform.position = new Vector3(137.2f, 2.27f, 104f);
        }
    }
    void FixedUpdate()
    {
        if (transform.position.y < threshold)
        {
            transform.position = new Vector3(137.2f, 2.27f, 104f);
        }

    }

}

