using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Enemy Stats")]
    public float health;
    public float damage;
    public bool isDead = false;

    //Attacking
    public float attackCooldown = 1.0f;
    public bool canAttack = true;

    //Different Enemy states
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();

        //Stopping distance to make it stop right before the user
        agent.stoppingDistance = 1f;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        // if (playerInAttackRange && playerInSightRange) AttackPlayer();

        if (health <= 0) DestroyEnemy();
    }

    // Enemy Behaviour 
    private void Patroling()
    {
        // if walkpoint is not set, search for one
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    // Enemy Behaviour
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    // Following the player
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    // Attacking script
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && canAttack)
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                //This is for debugging can be removed once final enemy script is done
                print("Player hit");
                playerHealth.TakeDamage(damage);
                StartCoroutine(AttackCooldown());
            }
        }
    }

    // Cooldown for attacking
    // Cooldown changed based on enemy type and damage
     private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Taking damage
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    // Enemy death
    private void DestroyEnemy()
    {
        Rigidbody rb = GetComponent<Rigidbody>(); 
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(transform.forward * 2f + Vector3.up * 2f, ForceMode.Impulse);

        isDead = true;

        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<EnemyAi>());
    }
}