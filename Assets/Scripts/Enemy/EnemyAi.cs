using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Enemy Stats")]
    public float health;
    public float damage;
    public bool isDead = false;

    // Attacking
    public float attackCooldown = 1.0f;
    public bool canAttack = true;

    // Different Enemy states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Animation")]
    public Animator animator;
    private int AttackNumber = 0;
    private string AnimationState = "Patrolling";
    private bool isAttacking = false;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();

        // Stopping distance to make it stop right before the player
        agent.stoppingDistance = 1f;
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();

        if (health <= 0) DestroyEnemy();

        handleAnimations();
    }

    // Enemy Behaviour Finite State Machine
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    // Attacking with triggers
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            isAttacking = true;
            StartCoroutine(PerformAttackWithDelay(other));
        }
    }

    private IEnumerator PerformAttackWithDelay(Collider playerCollider)
    {
        canAttack = false;

        // Trigger the attack animation immediately
        AttackNumber = (AttackNumber + 1) % 2;
        animator.SetTrigger(AttackNumber == 0 ? "PunchLeft" : "PunchRight");

        // Wait for a fraction of time to sync with animation
        yield return new WaitForSeconds(0.5f);  // Adjust this to match the animation frame where the punch lands

        PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);  // Apply damage after the animation delay
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;  // Stop attacking when player leaves trigger
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    //Allows the player to damage the enemy
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
        else animator.SetTrigger("Hit");
    }

    //The enemy death state
    private void DestroyEnemy()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(transform.forward * 2f + Vector3.up * 2f, ForceMode.Impulse);

        isDead = true;

        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<EnemyAi>());
        AnimationState = "Dead";
    }

    //The animation handlers 
    private void handleAnimations()
    {
        //Animations for when the enemy is dead
        if (isDead)
        {
            animator.SetBool("Alive", false);
            animator.SetBool("Chasing", false);
            animator.SetBool("InCombat", false);
            animator.SetBool("Patrolling", false);
            return;
        }

        //Animations for the enemy chasing the player
        if (playerInSightRange && !playerInAttackRange)
        {
            if (AnimationState != "Chasing")
            {
                AnimationState = "Chasing";
                animator.SetBool("Chasing", true);
                animator.SetBool("InCombat", false);
                animator.SetBool("Patrolling", false);
            }
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            // Setting animations for the enemy attacking the player
            if (AnimationState != "Attacking" && isAttacking)
            {
                AnimationState = "Attacking";
                animator.SetBool("InCombat", true);
                animator.SetBool("Chasing", false);
                animator.SetBool("Patrolling", false);
            }
        }
        else if (!playerInSightRange && AnimationState != "Dead")
        {
            // Setting the animation for the enemy patrolling
            if (AnimationState != "Patrolling")
            {
                AnimationState = "Patrolling";
                animator.SetBool("Patrolling", true);
                animator.SetBool("Chasing", false);
                animator.SetBool("InCombat", false);
            }
        }
    }
}
