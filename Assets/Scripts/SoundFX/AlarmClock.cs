using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmClock : MonoBehaviour
{
    AudioSource audioSource = null;
    SoundFXManager fXManager = null;
    MorningTask morningTask = null;

    private void Awake()
    {
        morningTask = GetComponent<MorningTask>();
        morningTask.onTaskComplete += TurnOff;
    }

    private void Start()
    {
        //audioSource =GetComponent<AudioSource>();
        //audioSource.loop = true;
        //audioSource.clip = alarmClock;

        //audioSource.transform.parent = transform;
        //audioSource.transform.localPosition = Vector3.zero;

        //audioSource.Play();
    }

    public void TurnOff()
    {
        audioSource.Stop();
    }
}
