using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    public GameObject Sword;
    public bool canAttack = true;
    public float Cooldown = 1.0f;
    public float attackRange = 1.5f;
    public float damage = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) 
        {
            Attack();
        }
    }

    public void Attack()
    {
        canAttack = false;
        Animator animator = Sword.GetComponent<Animator>();
        animator.SetTrigger("Attack");

        Collider[] hitColliders = Physics.OverlapSphere(Sword.transform.position, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyAi enemyScript = hitCollider.GetComponent<EnemyAi>();
                if (enemyScript != null)
                {
                    enemyScript.health -= damage;
                    if (enemyScript.health <= 0)
                    {
                        enemyScript.health = 0;
                    }
                }
            }
        }

        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        canAttack = true;
    }
}