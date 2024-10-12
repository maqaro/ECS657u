using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    public GameObject Sword;
    public bool canAttack = true;
    public float Cooldown = 1.0f;

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
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(Cooldown);
        canAttack = true;
    }
}
