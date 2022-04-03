using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerPhase { One, Two, Three }

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject birdObject = null;
    [SerializeField] Transform birdCamTransform = null;
    [SerializeField] AudioSource wingFlapSource = null;
    [SerializeField] float flySpeed = 20f;

    [SerializeField] PlayerPhase currentPhase = PlayerPhase.One;

    [SerializeField] LayerMask startingMask;

    RigidbodyFirstPersonController firstPersonController = null;
    Rigidbody rb = null;
    UICanvas uiCanvas = null;
    Fader fader = null;
    MusicPlayer musicPlayer = null; 
    SoundFXManager soundFXManager = null;

    CheckpointManager checkpointManager = null;
    MorningTasksSequence morningTasksSequence = null;

    Camera mainCam = null;
    Vector3 camStartPos = Vector3.zero;
    Quaternion camStartRot = Quaternion.identity;

    [SerializeField] AudioClip deathSound = null;
    [SerializeField] AudioClip jumpSound = null;
    [SerializeField] AudioClip landSound = null;

    [SerializeField] AudioClip[] footstepsClips = null;
    [SerializeField] float timeBetweenFootsteps = .5f;
    List<AudioSource> footstepsAudioSources = new List<AudioSource>();
    int footstepsIndex = 0;
    bool canPlayFootstepSound = false;
    bool hasBegunLandSound = false;

    Transform currentCheckpoint = null;
    RaycastableObject currentRaycastableObject = null;

    CharacterController characterController = null;
    Vector3 moveDirection = Vector3.zero;

    [SerializeField] bool isDead = false;
    bool canFly = false;

    private void Awake()
    {
        mainCam = Camera.main;

        firstPersonController = GetComponent<RigidbodyFirstPersonController>();
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        uiCanvas = FindObjectOfType<UICanvas>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
        fader = FindObjectOfType<Fader>();
        musicPlayer = FindObjectOfType<MusicPlayer>();

        checkpointManager = FindObjectOfType<CheckpointManager>();
        morningTasksSequence = FindObjectOfType<MorningTasksSequence>();

        firstPersonController.onJump += () => CreateJumpSound();
        firstPersonController.onJump += () => StartCoroutine(BeginJumpSound());

        uiCanvas.onControlsClose += () => SetPlayerPhase(PlayerPhase.One);
    }

    private void Start()
    {
        camStartPos = mainCam.transform.localPosition;
        camStartRot = mainCam.transform.localRotation;

        SetupFootstepsAudioSource();

        mainCam.cullingMask = startingMask;

        StartCoroutine(StartGame());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ActivateCursor(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (currentPhase == PlayerPhase.Two)
        {
            if (!canFly) return;
            FlyForward();
        }
        else
        { 
            HandleRaycasts();

            if (!canPlayFootstepSound) return;

            StartCoroutine(HandleFootsteps());
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

    private IEnumerator StartGame()
    {
        ActivateFirstPersonController(false);
        ActivateCursor(true);

        yield return new WaitForSeconds(1f);
        uiCanvas.ActivateControls();
    }

    private void HandleRaycasts()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            RaycastableObject raycast = hit.collider.GetComponentInParent<RaycastableObject>();

            currentRaycastableObject = raycast;
        }
        else return;

        if (currentRaycastableObject != null)
        {
            bool isInRange = (Vector3.Distance(transform.position, currentRaycastableObject.transform.position) < currentRaycastableObject.activationDistance);

            if (isInRange)
            {
                uiCanvas.ActivateActivationText(currentRaycastableObject.activationText);

                if (Input.GetMouseButtonDown(0))
                {
                    currentRaycastableObject.OnClick();
                }
            }
            else
            {
                uiCanvas.DeactivateActivationText();
            }
        }
        else
        {
            uiCanvas.DeactivateActivationText();
        }
    }

    public void StartFlying()
    {
        canFly = true;
        wingFlapSource.enabled = true;
        Vector3 lookPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        transform.LookAt(lookPosition);
    }

    private void FlyForward()
    {
        if (characterController.enabled == false) return;
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 1).normalized;
        characterController.Move(moveDirection * flySpeed * Time.deltaTime);
    }

    public IEnumerator Die()
    {
        if (!isDead)
        {
            isDead = true;
            yield return DeathBehavior();
        }
    }

    private IEnumerator DeathBehavior()
    {
        soundFXManager.CreateSoundFX(deathSound, transform, 1);

        if (fader != null)
        {
            yield return fader.FadeOut(1, Color.black, null);
        }

        checkpointManager.ResetToLastCheckpoint(currentPhase); 
        
        if (currentPhase != PlayerPhase.Two)
        {
            ActivateFirstPersonController(false);
        }
        else
        {
            characterController.enabled = false;
        }

        transform.position = currentCheckpoint.position;
        mainCam.transform.localEulerAngles = Vector3.zero;

        yield return new WaitForSeconds(1);

        if (currentPhase != PlayerPhase.Two)
        {
            ActivateFirstPersonController(true);
        }
        else
        {
            Vector3 lookPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
            transform.LookAt(lookPosition);
            characterController.enabled = true;
        }

        isDead = false;

        if (fader != null)
        {
            yield return fader.FadeIn(1);
        }          
    }

    private static void ActivateCursor(bool shouldActivate)
    {
        if (shouldActivate)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        Cursor.visible = shouldActivate;
    }

    public void ActivateFirstPersonController(bool shouldActivate)
    {
        firstPersonController.enabled = shouldActivate;
        rb.useGravity = shouldActivate;
        rb.isKinematic = !shouldActivate;
        canPlayFootstepSound = shouldActivate;

        if (shouldActivate)
        {
            mainCam.transform.localPosition = camStartPos;
            mainCam.transform.localRotation = camStartRot;
        }
    }

    public void SetPlayerPhase(PlayerPhase newPhase)
    {
        currentPhase = newPhase;

        StartCoroutine(HandlePhaseChanges());
    }

    public IEnumerator HandlePhaseChanges()
    {
        if (currentPhase == PlayerPhase.One)
        {
            birdObject.SetActive(false);
            ActivateFirstPersonController(true);
            characterController.enabled = false;
            canFly = false;
            morningTasksSequence.BeginMorningTasksSequence();
        }
        else if (currentPhase == PlayerPhase.Two)
        {
            ActivateFirstPersonController(false);

            currentCheckpoint = checkpointManager.GetPhase2Checkpoint();

            mainCam.transform.position = birdCamTransform.position;
            mainCam.transform.rotation = birdCamTransform.rotation;

            characterController.enabled = true;
            birdObject.SetActive(true);
            StartFlying();
        }
        else if (currentPhase == PlayerPhase.Three)
        {
            birdObject.SetActive(false);
            characterController.enabled = false;
            canFly = false;

            Transform phase3StartTransform = checkpointManager.GetPhase3StartTransform();
            transform.position = phase3StartTransform.position;
            transform.rotation = phase3StartTransform.rotation;

            currentCheckpoint = checkpointManager.GetPhase3Checkpoint();

            ActivateFirstPersonController(true);
        }

        musicPlayer.SetSong(currentPhase);

        if (fader == null) yield break;
        yield return fader.FadeIn(1);
    }

    public void UpdateCheckpoint(Transform _checkpoint)
    {
        currentCheckpoint = _checkpoint;
    }

    private void SetupFootstepsAudioSource()
    {
        canPlayFootstepSound = false;

        foreach(AudioClip footstepsSound in footstepsClips)
        {
            AudioSource newSource = soundFXManager.AssignNewAudioSource();
            newSource.playOnAwake = false;
            newSource.Stop();
            newSource.clip = footstepsSound;
            newSource.volume = .1f;
            newSource.transform.parent = transform;
            newSource.transform.localPosition = Vector3.zero;

            newSource.loop = false;

            footstepsAudioSources.Add(newSource);
        }
    }

    private IEnumerator HandleFootsteps()
    {
        float vaxis = Input.GetAxis("Vertical");
        float haxis = Input.GetAxis("Horizontal");

        bool input = vaxis > 0 || haxis > 0;

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

        if(footstepsIndex == footstepsAudioSources.Count)
        {
            footstepsIndex = 0;
        }

        return footstepsAudioSources[footstepsIndex];
    }

    private bool AreAnyFootstepsSourcesPlaying()
    {
        foreach(AudioSource audioSource in footstepsAudioSources)
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
        soundFXManager.CreateSoundFX(landSound, transform, .5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(currentPhase == PlayerPhase.Two)
        {
            LightSpeedSequence lightSpeedSequence = other.GetComponent<LightSpeedSequence>();

            if (lightSpeedSequence != null)
            {
                StartCoroutine(lightSpeedSequence.ActivateLightSpeedSequence(this));
                return;
            }

        }
        else if (currentPhase == PlayerPhase.Three)
        {
            SchoolChaseSequence schoolChaseSequence = other.GetComponent<SchoolChaseSequence>();

            if (schoolChaseSequence != null)
            {
                schoolChaseSequence.BeginChaseSequence();
                return;
            }
        }

        Whale hitWhale = other.GetComponentInParent<Whale>();
        OutOfBounds oob = other.GetComponent<OutOfBounds>();

        if (hitWhale != null || oob != null)
        {
            StartCoroutine(Die());
            return;
        }

        EndDoor endDoor = other.GetComponent<EndDoor>();

        if (endDoor != null)
        {
            StartCoroutine(endDoor.EndGame());
        }
    }
}