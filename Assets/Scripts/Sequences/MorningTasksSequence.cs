using System;
using System.Collections.Generic;
using UnityEngine;

public class MorningTasksSequence : MonoBehaviour
{
    [SerializeField] List<MorningTask> morningTasks = new List<MorningTask>();
    [SerializeField] Door doorToUnlock = null;

    MorningTask currentTask = null;

    int maxTasks = 0;

    private void Awake()
    {
        foreach(MorningTask morningTask in morningTasks)
        {
            morningTask.onTaskComplete += AdvanceTask;
        }

        maxTasks = morningTasks.Count;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            BeginMorningTasksSequence();
        }
    }

    public void BeginMorningTasksSequence()
    {
        SetCurrentTask(morningTasks[0]);
    }

    private void AdvanceTask()
    {
        int currentTaskIndex = morningTasks.IndexOf(currentTask);
        int newTaskIndex = currentTaskIndex + 1;

        if (newTaskIndex == maxTasks)
        {
            CompleteMorningTaskSequence();
            return;
        }
        else
        {
            SetCurrentTask(morningTasks[newTaskIndex]);
        }
    }

    public void SetCurrentTask(MorningTask morningTask)
    {
        if (currentTask != null)
        {
            currentTask.ActivateTask(false);
        }

        currentTask = morningTask;
        currentTask.ActivateTask(true);
    }

    private void CompleteMorningTaskSequence()
    {
        doorToUnlock.LockDoor(false);
        currentTask.HighlightTask(false);
    }
}
