using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBillboard : MonoBehaviour
{
    public Transform cam;

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
