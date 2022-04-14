using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class HumanSoundMaker : MonoBehaviour
{
    [SerializeField] HumanSounds startingSounds = null;
    [SerializeField] HumanSounds currentSounds = null;

    [SerializeField] AudioClip jumpSound = null;

    [SerializeField] float timeBetweenFootsteps = .5f;

    RigidbodyFirstPersonController firstPersonController = null;
    Rigidbody rb = null;
    SoundFXManager soundFXManager = null;

    List<AudioSource> footstepsAudioSources = new List<AudioSource>();
    int footstepsIndex = 0;
    bool canPlayFootstepSound = false;
    bool hasBegunLandSound = false;

    bool isActive = true;

    private void Awake()
    {
        firstPersonController = GetComponent<RigidbodyFirstPersonController>();
        rb = GetComponent<Rigidbody>();
        soundFXManager = FindObjectOfType<SoundFXManager>();

        firstPersonController.onJump += () => CreateJumpSound();
        firstPersonController.onJump += () => StartCoroutine(BeginJumpSound());
    }

    private void Start()
    {
        SetupFootstepsAudioSources();
        SetNewSounds(startingSounds);
        isActive = true;
        canPlayFootstepSound = true;
    }

    private void Update()
    {
        if (isActive)
        {
            if (canPlayFootstepSound)
            {
                StartCoroutine(HandleFootsteps());
            }
        }

        if (hasBegunLandSound)
        {
            if (firstPersonController.IsGrounded())
            {
                hasBegunLandSound = false;
                CreateLandSound();
            }
        }
    }

    private IEnumerator HandleFootsteps()
    {
        float vaxis = Input.GetAxis("Vertical");
        float haxis = Input.GetAxis("Horizontal");

        bool input = vaxis > 0 || haxis > 0 || vaxis < 0 || haxis < 0;

        if (input && rb.velocity.magnitude > 0 && firstPersonController.Grounded)
        {
            if (AreAnyFootstepsSourcesPlaying()) yield break;
            canPlayFootstepSound = false;

            AudioSource activeSource = GetNextFootstepSource();
            activeSource.Play();

            yield return new WaitForSeconds(timeBetweenFootsteps);

            activeSource.Stop();
            canPlayFootstepSound = true;
        }
    }
    private AudioSource GetNextFootstepSource()
    {
        footstepsIndex++;

        if (footstepsIndex == footstepsAudioSources.Count)
        {
            footstepsIndex = 0;
        }

        return footstepsAudioSources[footstepsIndex];
    }

    private bool AreAnyFootstepsSourcesPlaying()
    {
        foreach (AudioSource audioSource in footstepsAudioSources)
        {
            if (audioSource.isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    private void CreateJumpSound()
    {
        soundFXManager.CreateSoundFX(jumpSound, transform, .5f);
    }

    private IEnumerator BeginJumpSound()
    {
        yield return new WaitForSeconds(.2f);
        hasBegunLandSound = true;
    }

    private void CreateLandSound()
    {
        soundFXManager.CreateSoundFX(currentSounds.GetLanding(), transform, .5f);
    }

    public bool CanPlayFootstepsSound()
    {
        return canPlayFootstepSound;
    }

    private void SetupFootstepsAudioSources()
    {
        canPlayFootstepSound = false;

        for (int i = 0; i < 2; i++)
        {
            AudioSource newSource = soundFXManager.AssignNewAudioSource();
            newSource.playOnAwake = false;
            newSource.Stop();
            newSource.volume = .75f;
            newSource.transform.parent = transform;
            newSource.transform.localPosition = Vector3.zero;

            newSource.loop = false;

            footstepsAudioSources.Add(newSource);
        } 
    }

    public void SetNewSounds(HumanSounds newSounds)
    {
        currentSounds = newSounds;

        for (int i = 0; i < currentSounds.GetFootstepsClips().Length; i++)
        {
            footstepsAudioSources[i].clip = currentSounds.GetFootstepsClips()[i];
            footstepsAudioSources[i].volume = newSounds.GetFootstepsVolume();
        }
    }

    public void ActivateSoundMaker(bool shouldActivate)
    {
        isActive = shouldActivate;
    }
}

[Serializable]
public class HumanSounds
{
    [SerializeField] AudioClip[] footsteps = null;
    [Range(0,1)][SerializeField] float footstepsVolume = 1f;
    [SerializeField] AudioClip landing = null;

    public AudioClip[] GetFootstepsClips()
    {
        return footsteps;
    }

    public float GetFootstepsVolume()
    {
        return footstepsVolume;
    }

    public AudioClip GetLanding()
    {
        return landing;
    }
}