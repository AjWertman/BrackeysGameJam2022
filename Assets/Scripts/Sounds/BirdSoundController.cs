using UnityEngine;

public class BirdSoundController : MonoBehaviour
{
    AudioSource audioSource = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Flap()
    {
        audioSource.Play();
    }
}
