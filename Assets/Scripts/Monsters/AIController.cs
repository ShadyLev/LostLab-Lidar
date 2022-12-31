using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent agent;
    private NavMeshPath navPath;

    [Header("Player references")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] LayerMask whatIsPlayer;

    [Header("General values")]
    [Range(0, 100)]
    [SerializeField] float sightRange;
    [Range(0, 100)]
    [SerializeField] float attackRange;
    [SerializeField] bool playerInSightRange;
    [SerializeField] bool playerInAttackRange;

    [Header("Patrolling")]
    [SerializeField] Vector3 walkPoint;
    [Range(0, 100)]
    [SerializeField] float walkPointRange;
    [SerializeField] bool walkPointSet;

    [Header("Attacking")]
    [Range(0, 1)]
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] bool alreadyAttacked;


    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        navPath = new NavMeshPath();
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

    /// <summary>
    /// Sets destination to a random point in radius.
    /// </summary>
    void Patrolling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        agent.SetDestination(walkPoint);

        Vector3 distanceToDestination = transform.position - walkPoint;

        if (distanceToDestination.magnitude < 1f)
            walkPointSet = false;
    }

    /// <summary>
    /// Finds a random point on a navmesh in radius
    /// </summary>
    void SearchWalkPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkPointRange, NavMesh.AllAreas);
        Vector3 finalPosition = hit.position;

        if (CheckIfPathPossible(finalPosition))
        {
            walkPoint = finalPosition;
            walkPointSet = true;
        }
    }

    bool CheckIfPathPossible(Vector3 targetPosition)
    {
        agent.CalculatePath(targetPosition, navPath);

        if(navPath.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Chasing

    /// <summary>
    /// Sets destination to player.
    /// </summary>
    void Chasing()
    {
        agent.SetDestination(playerTransform.position);
    }

    #endregion

    #region Attacking

    /// <summary>
    /// Attacks the player in intervals.
    /// </summary>
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

    /// <summary>
    /// Resets the attack bool.
    /// </summary>
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
        Gizmos.DrawWireSphere(transform.position, walkPointRange);

        // Chase range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #endregion
}
