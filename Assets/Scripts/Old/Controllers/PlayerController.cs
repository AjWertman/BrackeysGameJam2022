using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerPhase { One, Two, Three }

public class PlayerController : MonoBehaviour
{
    HumanSoundMaker humanSoundMaker = null;

    [SerializeField] GameObject birdObject = null;
    [SerializeField] Transform birdCamTransform = null;
    [SerializeField] AudioSource wingFlapSource = null;
    [SerializeField] float flySpeed = 20f;
    [SerializeField] float pitchFactor = -2f;
    [SerializeField] float yawFactor = -20f;
    [SerializeField] float rollFactor = -3f;

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

    Transform currentCheckpoint = null;
    RaycastableObject currentRaycastableObject = null;

    CharacterController characterController = null;
    Vector3 moveDirection = Vector3.zero;

    [SerializeField] bool isDead = false;
    bool canFly = false;

    bool isTransitioningCam = false;

    private void Awake()
    {
        mainCam = Camera.main;

        humanSoundMaker = GetComponent<HumanSoundMaker>();

        firstPersonController = GetComponent<RigidbodyFirstPersonController>();
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        uiCanvas = FindObjectOfType<UICanvas>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
        fader = FindObjectOfType<Fader>();
        musicPlayer = FindObjectOfType<MusicPlayer>();

        checkpointManager = FindObjectOfType<CheckpointManager>();
        morningTasksSequence = FindObjectOfType<MorningTasksSequence>();


        uiCanvas.onControlsClose += () => SetPlayerPhase(PlayerPhase.One);
    }

    private void Start()
    {
        camStartPos = mainCam.transform.localPosition;
        camStartRot = mainCam.transform.localRotation;

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
        }

        if (isTransitioningCam)
        {
            mainCam.transform.position = Vector3.MoveTowards(mainCam.transform.position, birdCamTransform.position, 10 * Time.deltaTime);
            //mainCam.transform.rotation = Quaternion.RotateTowards(mainCam.transform.rotation, birdCamTransform.localRotation, 400 * Time.deltaTime);

            mainCam.transform.eulerAngles = Vector3.MoveTowards(mainCam.transform.eulerAngles, birdCamTransform.eulerAngles, 400 * Time.deltaTime);

            if (mainCam.transform.position == birdCamTransform.position && mainCam.transform.eulerAngles == birdCamTransform.eulerAngles)
            {
                isTransitioningCam = false;
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
        Vector3 lookPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        transform.LookAt(lookPosition);
    }

    private void FlyForward()
    {
        if (characterController.enabled == false) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontalInput, verticalInput , 1).normalized;
        characterController.Move(moveDirection * flySpeed * Time.deltaTime);

        float pitchDirection = moveDirection.y * pitchFactor;
        float yawDirection = moveDirection.x * yawFactor;
        float rollDirection = moveDirection.x * rollFactor;

        float pitch =  pitchDirection;
        float yaw = yawDirection;
        float roll = rollDirection;

        birdObject.transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
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
            yield return fader.FadeOut(.5f, Color.black, null);
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
       

        yield return new WaitForSeconds(1);

        if (currentPhase != PlayerPhase.Two)
        {
            mainCam.transform.localEulerAngles = Vector3.zero;
            ActivateFirstPersonController(true);
        }
        else
        {
            Vector3 lookPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
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
        humanSoundMaker.ActivateSoundMaker(shouldActivate);

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

            isTransitioningCam = true;          

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

    private void OnTriggerEnter(Collider other)
    {
        if(currentPhase == PlayerPhase.One)
        {
            BirdTransformation birdTransformation = other.GetComponent<BirdTransformation>();

            if(birdTransformation != null)
            {
                rb.isKinematic = true;
                StartCoroutine(birdTransformation.BeginBirdTransformation());
            }
        }
        else if(currentPhase == PlayerPhase.Two)
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

        HumanSoundsTrigger humanSoundsTrigger = other.GetComponent<HumanSoundsTrigger>();

        if(humanSoundsTrigger != null)
        {
            humanSoundMaker.SetNewSounds(humanSoundsTrigger.GetHumanSounds());
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