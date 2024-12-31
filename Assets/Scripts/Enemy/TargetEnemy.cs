using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEnemy : MonoBehaviour
{
    public int health;

    [SerializeField] private AudioClip[] hitSounds;

    public void TakeDamage(int damage)
    {
        SoundFXManager.instance.PlayRandomSoundFXClip(hitSounds, transform, 0.3f);
        health -= damage;

        if (health <= 0)
            Destroy(gameObject);
    }

}
