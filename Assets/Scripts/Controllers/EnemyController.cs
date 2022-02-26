using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    PlayerController playerController = null;
    Transform playerTransform = null;

    NavMeshAgent navMeshAgent = null;
    float caughtDistance = 0;

    bool isActivated = false;

    Vector3 startPosition = Vector3.zero;
    Quaternion startRotation = Quaternion.identity;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerTransform = playerController.transform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        caughtDistance = navMeshAgent.stoppingDistance;
    }

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isActivated) return;

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);

        if(distanceToPlayer <= caughtDistance)
        {
            StartCoroutine(playerController.Die());      
        }
        else
        {
            navMeshAgent.SetDestination(playerController.transform.position);
        }
    }

    public void ResetEnemy()
    {
        print("reseting enemy"); 
        isActivated = false;
        navMeshAgent.enabled = false;
        transform.position = startPosition;
        transform.rotation = startRotation;
        navMeshAgent.enabled = true;
    }

    public void SetIsActivated(bool _isActivated)
    {
        isActivated = _isActivated;
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}
