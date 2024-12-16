using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiPickup : MonoBehaviour
{
    public int kunaiAmount = 20; // Number of kunai this pickup adds

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Throwing throwingScript = other.GetComponent<Throwing>();

            if (throwingScript != null)
            {
                throwingScript.AddThrows(kunaiAmount);
                Destroy(gameObject);
            }
        }
    }
}
