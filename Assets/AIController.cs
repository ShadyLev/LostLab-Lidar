using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private Transform playerTransform;

    [SerializeField] LayerMask whatIsGround;
    [SerializeField] LayerMask whatIsPlayer;

    [Header("Patrolling")]
    [SerializeField] Vector3 walkPoint;
    [SerializeField] float walkPointRange;
    [SerializeField] bool walkPointSet;

    [Header("Attacking")]
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] bool alreadyAttacked;

    [Header("General values")]
    [SerializeField] float sightRange;
    [SerializeField] float attackRange;
    [SerializeField] bool playerInSightRange;
    [SerializeField] bool playerInAttackRange;


    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patrolling();
        if (playerInSightRange && !playerInAttackRange)
            Chasing();
        if (playerInSightRange && playerInAttackRange)
            Attacking();
    }

    #region Patrolling
    void Patrolling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        agent.SetDestination(walkPoint);

        Vector3 distanceToDestination = transform.position - walkPoint;

        if (distanceToDestination.magnitude < 1f)
            walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    #endregion

    #region Chasing

    void Chasing()
    {
        agent.SetDestination(playerTransform.position);
    }

    #endregion

    #region Attacking

    void Attacking()
    {
        // Stop
        agent.SetDestination(transform.position);

        // Look at player
        transform.LookAt(playerTransform);

        if (!alreadyAttacked)
        {
            // here


            alreadyAttacked = true;

            Invoke("ResetAttack", timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        // Patrolling range
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, walkPointRange);

        // Chase range
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, sightRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, attackRange);
    }

    #endregion
}
