using UnityEngine;

public class AlarmClock : MonoBehaviour
{
    [SerializeField] AudioClip alarmClockSound = null;

    AudioSource audioSource = null;
    SoundFXManager fXManager = null;
    MorningTask morningTask = null;

    private void Awake()
    {
        morningTask = GetComponent<MorningTask>();
        morningTask.onTaskComplete += TurnOff;

        fXManager = FindObjectOfType<SoundFXManager>();
    }

    private void Start()
    {
        audioSource = fXManager.AssignNewAudioSource();
        audioSource.clip = alarmClockSound;
        BeginAlarmClock();
    }

    public void BeginAlarmClock()
    {
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
