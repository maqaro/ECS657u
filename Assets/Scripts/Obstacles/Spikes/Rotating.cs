using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{

    public float speed = 10f;

    void Update () {
        transform.Rotate (0,0,50*Time.deltaTime);
    }
}
