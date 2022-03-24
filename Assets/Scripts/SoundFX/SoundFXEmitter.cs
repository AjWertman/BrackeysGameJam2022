using System;
using UnityEngine;

public class SoundFXEmitter: MonoBehaviour
{
    [SerializeField] AudioClip clip = null;

    public event Action<AudioClip> onSFXTriggered;

    public void PlayClip()
    {
        onSFXTriggered(clip);
    }

    public AudioClip GetClip()
    {
        return clip;
    }
}
