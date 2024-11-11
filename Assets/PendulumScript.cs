using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumScript : MonoBehaviour
{
    public float speed = 2.5f;
    public float limit = 75f;
    public bool randomStart = false;
    private float random = 0;

    void Awake()
    {
        if (randomStart)
            random = Random.Range(0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        float angle = limit * Mathf.Sin(Time.time * speed);
        transform.localRotation = Quaternion.Euler(0, 90, angle);
    }
}
