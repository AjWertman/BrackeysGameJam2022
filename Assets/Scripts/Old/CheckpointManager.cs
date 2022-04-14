using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] Transform phase2Checkpoint = null;

    [Header("Phase Three")]
    [SerializeField] EnemyController enemy = null;
    [SerializeField] Door doorsToClosePhase3 = null;
    [SerializeField] Transform playerStartTransform3 = null;
    [SerializeField] Transform checkpointPhase3 = null;

    PlayerController playerController = null;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public Transform GetPhase2Checkpoint()
    {
        return phase2Checkpoint;
    }

    public void ResetToLastCheckpoint(PlayerPhase phase)
    {
        if (phase == PlayerPhase.Three)
        {
            ResetClassroomScene();
        }
    }

    private void ResetClassroomScene()
    {
        enemy.ResetEnemy();
        doorsToClosePhase3.CloseDoor();
    }

    public Transform GetPhase3StartTransform()
    {
        return playerStartTransform3;
    }

    public Transform GetPhase3Checkpoint()
    {
        return checkpointPhase3;
    }
}
