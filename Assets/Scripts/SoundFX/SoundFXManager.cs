using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] GameObject soundFXSourcePrefab = null;
    [SerializeField] int amountOfObjectsToPool = 20;

    Dictionary<AudioSource, bool> soundFXSourceInstances = new Dictionary<AudioSource, bool>();

    private void Start()
    {
        CreateSoundFXObjects();
    }

    public void CreateSoundFX(AudioClip clip, Transform clipLocation)
    {
        StartCoroutine(CreateSoundFXCoroutine(clip, clipLocation));
    }

    public IEnumerator CreateSoundFXCoroutine(AudioClip clip, Transform clipLocation)
    {
        if (clipLocation == null)
        {
            clipLocation = Camera.main.transform;
        }

        AudioSource availableAudioSource = GetAvailableAudioSource();
        soundFXSourceInstances[availableAudioSource] = true;

        float clipLength = clip.length;

        availableAudioSource.clip = clip;

        availableAudioSource.transform.parent = clipLocation;
        availableAudioSource.transform.localPosition = Vector3.zero;

        availableAudioSource.Play();

        yield return new WaitForSeconds(clipLength);

        soundFXSourceInstances[availableAudioSource] = false;
        availableAudioSource.clip = null;
        availableAudioSource.transform.parent = transform;
        availableAudioSource.transform.localPosition = Vector3.zero;
    }

    private void CreateSoundFXObjects()
    {
        for (int i = 0; i < amountOfObjectsToPool; i++)
        {
            GameObject newSourceInstance = Instantiate(soundFXSourcePrefab, transform);
            AudioSource audioSource = newSourceInstance.GetComponent<AudioSource>();

            soundFXSourceInstances.Add(audioSource, false);
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        AudioSource availableAudioSource = null;

        foreach(AudioSource audioSource in soundFXSourceInstances.Keys)
        {
            if (soundFXSourceInstances[audioSource]) continue;

            availableAudioSource = audioSource;
            break;
        }

        return availableAudioSource;
    }
}
