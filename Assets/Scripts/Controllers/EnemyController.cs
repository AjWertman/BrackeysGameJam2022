using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    PlayerController playerController = null;
    Transform playerTransform = null;

    NavMeshAgent navMeshAgent = null;
    float caughtDistance = 0;

    bool isActivated = false;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerTransform = playerController.transform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        caughtDistance = navMeshAgent.stoppingDistance;
    }

    private void Update()
    {
        if (!isActivated) return;

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);

        if(distanceToPlayer > caughtDistance)
        {
            navMeshAgent.SetDestination(playerController.transform.position);
        }
        else
        {
            //ScreenFX 
            //Reset
        }
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
