using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerPhase { One, Two, Three }

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject camObject = null;

    [SerializeField] GameObject birdObject = null;
    [SerializeField] Transform birdCamTransform = null;
    [SerializeField] float flySpeed = 20f;

    [SerializeField] Transform phase3StartTransform = null;

    [SerializeField] PlayerPhase phase = PlayerPhase.One;

    [SerializeField] AudioClip alarmClock = null;

    Camera mainCam = null;
    Vector3 camStartPos = Vector3.zero;
    Quaternion camStartRot = Quaternion.identity;

    RigidbodyFirstPersonController firstPersonController = null;
    Rigidbody rb = null;
    UICanvas uiCanvas = null;
    Fader fader = null;
    MusicPlayer musicPlayer = null;

    CheckpointManager checkpointManager = null;
    MorningTasksSequence morningTasksSequence = null;
    RaycastableObject currentRaycastableObject = null;
    SoundFXManager soundFXManager = null;

    CharacterController characterController = null;
    Vector3 moveDirection = Vector3.zero;

    [SerializeField] bool isDead = false;
    bool canFly = false;

    private void Awake()
    {
        mainCam = camObject.GetComponent<Camera>();
        firstPersonController = GetComponent<RigidbodyFirstPersonController>();
        characterController = GetComponent<CharacterController>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
        rb = GetComponent<Rigidbody>();
        uiCanvas = FindObjectOfType<UICanvas>();
        fader = FindObjectOfType<Fader>();
        musicPlayer = FindObjectOfType<MusicPlayer>();

        checkpointManager = FindObjectOfType<CheckpointManager>();
        morningTasksSequence = FindObjectOfType<MorningTasksSequence>();
        uiCanvas.onControlsClose += () => StartCoroutine(SetNewPhase(PlayerPhase.One));
    }

    private void Start()
    {
        camStartPos = mainCam.transform.localPosition;
        camStartRot = mainCam.transform.localRotation;

        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        ActivateFirstPersonController(false);
        ActivateCursor(true);
        yield return soundFXManager.CreateSoundFXCoroutine(alarmClock, null);
        yield return fader.FadeIn(1);

        yield return new WaitForSeconds(1f);
        uiCanvas.ActivateControls();
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

        if(phase == PlayerPhase.Two)
        {
            if (!canFly) return;
            Fly();
        }

        HandleRaycasts();
    }

    public void SetPlayerPhase(PlayerPhase newPhase)
    {
        phase = newPhase;
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

    private void Fly()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 1).normalized;
        characterController.Move(moveDirection * flySpeed * Time.deltaTime);
    }

    public void StartFlying()
    {
        canFly = true;
        Vector3 lookPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
        transform.LookAt(lookPosition);
    }

    public IEnumerator SetNewPhase(PlayerPhase _phase)
    {
        phase = _phase;

        if (phase == PlayerPhase.One)
        {       
            birdObject.SetActive(false);
            ActivateFirstPersonController(true);
            characterController.enabled = false;
            canFly = false;
            morningTasksSequence.BeginMorningTasksSequence();
        }
        else if (phase == PlayerPhase.Two)
        {
            ActivateFirstPersonController(false);

            mainCam.transform.position = birdCamTransform.position;
            mainCam.transform.rotation = birdCamTransform.rotation;

            characterController.enabled = true;
            birdObject.SetActive(true);
            StartFlying();
        }
        else if (phase == PlayerPhase.Three)
        {           
            birdObject.SetActive(false);
            characterController.enabled = false;
            canFly = false;

            transform.position = phase3StartTransform.position;
            transform.rotation = phase3StartTransform.rotation;

            ActivateFirstPersonController(true);

            fader.GetComponent<CanvasGroup>().alpha = 0;
        }

        musicPlayer.SetSong(phase);

        yield return fader.FadeIn(1);
    }

    public void ActivateFirstPersonController(bool shouldActivate)
    {
        firstPersonController.enabled = shouldActivate;
        rb.useGravity = shouldActivate;
        rb.isKinematic = !shouldActivate;

        if (shouldActivate)
        {
            mainCam.transform.localPosition = camStartPos;
            mainCam.transform.localRotation = camStartRot;
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
        yield return fader.FadeOut(1, Color.black, null);

        if (phase != PlayerPhase.Two)
        {
            ActivateFirstPersonController(false);
        }
        else
        {
            characterController.enabled = false;
        }

        checkpointManager.ResetToLastCheckpoint(phase);
        Transform checkpointTransform = checkpointManager.GetCheckpointPosition(phase);

        transform.position = checkpointTransform.position;
        transform.rotation = checkpointTransform.rotation;

        yield return new WaitForSeconds(1);

        if (phase != PlayerPhase.Two)
        {
            ActivateFirstPersonController(true);           
        }
        else
        {
            Vector3 lookPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            transform.LookAt(lookPosition);
            characterController.enabled = true;
        }
    
        isDead = false;

        yield return fader.FadeIn(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(phase == PlayerPhase.One)
        {
            Level1CheckpointTrigger level1CheckpointTrigger = other.GetComponent<Level1CheckpointTrigger>();

            if(level1CheckpointTrigger != null)
            {
                checkpointManager.ActivateNextLevel1Checkpoint();
                level1CheckpointTrigger.gameObject.SetActive(false);
            }
        }
        else if(phase == PlayerPhase.Two)
        {
            LightSpeedSequence lightSpeedSequence = other.GetComponent<LightSpeedSequence>();

            if (lightSpeedSequence != null)
            {
                StartCoroutine(Phase3Transition(lightSpeedSequence));
                return;
            }

        }
        else if (phase == PlayerPhase.Three)
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
        }              
    }

    private IEnumerator Phase3Transition(LightSpeedSequence lightSpeedSequence)
    {
        StartCoroutine(lightSpeedSequence.ActivateLightSpeedSequence(mainCam));
        yield return null;
        StartCoroutine(SetNewPhase(PlayerPhase.Three));
    }
}
