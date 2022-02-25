using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolChaseSequence : MonoBehaviour
{
    [SerializeField] EnemyController enemy = null;
    [SerializeField] Door enemyDoor = null;

    public void BeginChaseSequence()
    {
        enemyDoor.OpenDoor(true);
        enemy.SetIsActivated(true);
    }
}
