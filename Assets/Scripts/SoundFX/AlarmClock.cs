using UnityEngine;

public class AlarmClock : MonoBehaviour
{
    AudioSource audioSource = null;
    //SoundFXManager fXManager = null;
    MorningTask morningTask = null;

    private void Awake()
    {
        morningTask = GetComponent<MorningTask>();
        morningTask.onTaskComplete += TurnOff;
    }

    public void BeginAlarmClock()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        audioSource.transform.parent = transform;
        audioSource.transform.localPosition = Vector3.zero;

        audioSource.Play();
    }

    public void TurnOff()
    {
        audioSource.Stop();
    }
}
