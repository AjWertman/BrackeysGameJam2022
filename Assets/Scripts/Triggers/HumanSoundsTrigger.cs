using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSoundsTrigger : MonoBehaviour
{
    [SerializeField] HumanSounds sounds = null;

    public HumanSounds GetHumanSounds()
    {
        return sounds;
    }
}
