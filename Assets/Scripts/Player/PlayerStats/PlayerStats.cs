using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private float baseSpeed;
    public SwordSwing swordSwing;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        baseSpeed = playerMovement.walkSpeed;
        swordSwing = GetComponent<SwordSwing>();
    }

    public void ActivatePowerUp(PowerUpType type, float multiplier, float duration)
    {
        if (type == PowerUpType.Speed)
        {
            StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
        }
        else if (type == PowerUpType.Damage)
        {
            StartCoroutine(DamageBoostCoroutine(multiplier, duration));
        }
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        // Apply speed boost
        playerMovement.walkSpeed *= multiplier;
        playerMovement.sprintSpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        // Reset speed to normal
        playerMovement.walkSpeed = baseSpeed;
        playerMovement.sprintSpeed = baseSpeed * 1.5f;
    }

    private IEnumerator DamageBoostCoroutine(float multiplier, float duration)
    {
        // Store original damage values
        float originalDamage = swordSwing.damage;
        
        // Apply damage boost
        swordSwing.damage *= multiplier;

        yield return new WaitForSeconds(duration);

        // Reset damage to normal
        swordSwing.damage = originalDamage;
    }
}
