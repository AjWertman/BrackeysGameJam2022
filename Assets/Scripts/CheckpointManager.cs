using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Phase One")]
    [SerializeField] Transform playerStartTransform1_1 = null;
    [SerializeField] Transform playerStartTransform1_2 = null;
    Transform currentLevel1Transform = null;

    [Header("Phase Two")]
    [SerializeField] Transform playerStartTransform2 = null;

    [Header("Phase Three")]
    [SerializeField] EnemyController enemy = null;
    [SerializeField] Door[] doorsToClose3 = null;
    [SerializeField] Transform playerStartTransform3 = null;

    private void Start()
    {
        currentLevel1Transform = playerStartTransform1_1;
    }

    public void ResetToLastCheckpoint(PlayerPhase phase)
    {
        if (phase == PlayerPhase.One)
        {
            ResetToPhase1();
        }
        else if (phase == PlayerPhase.Two)
        {
            ResetToPhase2();
        }
        else if (phase == PlayerPhase.Three)
        {
            ResetToPhase3();
        }
    }

    private void ResetToPhase1()
    {
        //Nothing
    }

    private void ResetToPhase2()
    {
        //Nothing
    }

    private void ResetToPhase3()
    {        
        enemy.ResetEnemy();
        CloseDoors(doorsToClose3);
    }

    public Transform GetCheckpointPosition(PlayerPhase playerPhase)
    {
        Transform restartTransform = null;

        if (playerPhase == PlayerPhase.One)
        {
            restartTransform = currentLevel1Transform;
        }
        else if(playerPhase == PlayerPhase.Two)
        {
            restartTransform = playerStartTransform2;
        }
        else if (playerPhase == PlayerPhase.Three)
        {
            restartTransform = playerStartTransform3;
        }

        return restartTransform;
    }

    public void CloseDoors(Door[] doors)
    {
        foreach (Door door in doors)
        {
            door.OpenDoor(false);
        }
    }

    public void ActivateNextLevel1Checkpoint()
    {
        currentLevel1Transform = playerStartTransform1_2;
    }
}
