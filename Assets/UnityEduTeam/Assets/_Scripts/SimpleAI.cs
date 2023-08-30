using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SimpleAI : MonoBehaviour {
    [Header("Agent Field of View Properties")]
    public float viewRadius;
    public float viewAngle;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    [Space(5)]
    [Header("Agent Properties")]
    public float runSpeed;
    public float walkSpeed;
    public float patrolRadius;


    private Transform playerTarget;
    private Vector3 currentDestination;
    private bool playerSeen;
    private int maxNumberOfNewDestinationBeforeDeath;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private GameObject player;
    
    private enum State { Wandering, Chasing };
    private State currentState;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        currentDestination = RandomNavSphere(transform.position, patrolRadius, -1);
        maxNumberOfNewDestinationBeforeDeath = Random.Range(5, 50);
    }

    private void Update()
    {
        CheckState();

        if (playerSeen)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Wandering;
        }

        if (HasFoundPlayer())
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    private void CheckState()
    {
        FindVisibleTargets();

        switch (currentState)
        {
            case State.Chasing:
                ChaseBehavior();
                break;

            default:
                WanderBehavior();
                break;
        }
    }

    private void WanderBehavior()
    {
        animator.SetTrigger("walk");
        navMeshAgent.speed = walkSpeed;
        float dist = navMeshAgent.remainingDistance;

        if (dist != Mathf.Infinity && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            currentDestination = RandomNavSphere(transform.position, patrolRadius, -1);
            navMeshAgent.SetDestination(currentDestination);
            maxNumberOfNewDestinationBeforeDeath--;
            if (maxNumberOfNewDestinationBeforeDeath <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ChaseBehavior()
    {
        if (playerTarget != null)
        {
            animator.SetTrigger("run");
            navMeshAgent.speed = runSpeed;
            currentDestination = playerTarget.transform.position;
            navMeshAgent.SetDestination(currentDestination);
        }
        else
        {
            playerSeen = false;
            currentState = State.Wandering;
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.tag == "Player")
    //     {
    //     }
    // }

    #region Vision
    void FindVisibleTargets()
    {

        playerTarget = null;
        playerSeen = false;
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        
        if (players.Length == 0)
        {
            return;
        }
        else
        {
            //Debug.Log("Found Player");
        }

        foreach (GameObject player in players)
        {
            Vector3 dirToTarget = (player.transform.position - transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirToTarget, out hit))
            {
                float dstToTarget = Vector3.Distance(transform.position, player.transform.position);
                if (dstToTarget <= viewRadius)
                {
                    if (Vector3.Angle(transform.forward, dirToTarget) <= viewAngle / 2)
                    {
                        if (hit.collider.tag == "Player")
                        {
                            playerSeen = true;
                            playerTarget = hit.transform;
                        }
                    }
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    #endregion

    private bool HasFoundPlayer()
    {
        if (player == null)
        {
            return false;
        }

        return Vector3.Distance(player.transform.position, transform.position) <= navMeshAgent.radius * 2;
    }

}