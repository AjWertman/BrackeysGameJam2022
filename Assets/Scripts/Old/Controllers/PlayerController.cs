using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerPhase { One, Two, Three }

public class PlayerController : MonoBehaviour
{
    [SerializeField] AudioClip deathSound = null;

    [SerializeField] BirdTrigger birdTrigger = null;
    [SerializeField] LayerMask startingMask;

    PlayerPhase currentPhase = PlayerPhase.One;

    RigidbodyFirstPersonController firstPersonController = null;
    Rigidbody rb = null;
    BirdController birdController = null;
    UICanvas uiCanvas = null;
    Fader fader = null;
    MusicPlayer musicPlayer = null;
    SoundFXManager soundFXManager = null;
    HumanSoundMaker humanSoundMaker = null;
    CheckpointManager checkpointManager = null;
    MorningTasksSequence morningTasksSequence = null;

    Camera mainCam = null;
    Vector3 camStartPos = Vector3.zero;
    Quaternion camStartRot = Quaternion.identity;

    Transform currentCheckpoint = null;
    RaycastableObject currentRaycastableObject = null;

    bool isDead = false;

    private void Awake()
    {
        mainCam = Camera.main;

        humanSoundMaker = GetComponent<HumanSoundMaker>();
        birdController = GetComponent<BirdController>();
        birdTrigger.onDeath += () => StartCoroutine(DeathBehavior());

        firstPersonController = GetComponent<RigidbodyFirstPersonController>();
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

        if(currentPhase != PlayerPhase.Two)
        { 
            HandleRaycasts();
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
        if (fader != null)
        {
            yield return fader.FadeOut(.5f, Color.black, null);
        }

        if (currentPhase != PlayerPhase.Two)
        {
            soundFXManager.CreateSoundFX(deathSound, transform, 1);
            checkpointManager.ResetToLastCheckpoint(currentPhase);

            ActivateFirstPersonController(false);

            transform.position = currentCheckpoint.position;

            yield return new WaitForSeconds(1f);

            ActivateFirstPersonController(true);
        }
        else
        {
            StartCoroutine(birdController.BirdDeath(currentCheckpoint.position));
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
            birdController.Deactivate();
            ActivateFirstPersonController(true);
            morningTasksSequence.BeginMorningTasksSequence();
        }
        else if (currentPhase == PlayerPhase.Two)
        {
            ActivateFirstPersonController(false);

            currentCheckpoint = checkpointManager.GetPhase2Checkpoint();

            birdController.Activate();
        }
        else if (currentPhase == PlayerPhase.Three)
        {
            birdController.Deactivate();

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

        OutOfBounds oob = other.GetComponent<OutOfBounds>();

        if (oob != null)
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