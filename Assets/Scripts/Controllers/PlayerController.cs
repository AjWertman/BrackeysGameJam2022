using System;
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
    CapsuleCollider capsuleCollider = null;
    BirdController birdController = null;
    UICanvas uiCanvas = null;
    Fader fader = null;
    MusicPlayer musicPlayer = null;
    SoundFXManager soundFXManager = null;
    HumanSoundMaker humanSoundMaker = null;
    CheckpointManager checkpointManager = null;

    Camera mainCam = null;
    Vector3 camStartPos = Vector3.zero;
    Quaternion camStartRot = Quaternion.identity;

    Transform currentCheckpoint = null;
    RaycastableObject currentRaycastableObject = null;

    bool isDead = false;

    public event Action<PlayerPhase> onPhaseChange;

    private void Awake()
    {
        mainCam = Camera.main;

        humanSoundMaker = GetComponent<HumanSoundMaker>();
        birdController = GetComponent<BirdController>();
        birdTrigger.onDeath += () => StartCoroutine(DeathBehavior());

        firstPersonController = GetComponent<RigidbodyFirstPersonController>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        uiCanvas = FindObjectOfType<UICanvas>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
        fader = FindObjectOfType<Fader>();
        musicPlayer = FindObjectOfType<MusicPlayer>();

        checkpointManager = FindObjectOfType<CheckpointManager>();

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
        bool isCurrentPhaseTwo = currentPhase == PlayerPhase.Two;
        AudioClip deathClip = deathSound;
        if (isCurrentPhaseTwo)
        {
            deathClip = birdController.GetDeathSound();
            birdController.Deactivate();
        }

        soundFXManager.CreateSoundFX(deathClip, mainCam.transform, 1f);
        if (fader != null)
        {
            yield return fader.FadeOut(.5f, Color.black, null);
        }

        if (!isCurrentPhaseTwo)
        {         
            checkpointManager.ResetToLastCheckpoint(currentPhase);

            ActivateFirstPersonController(false);

            transform.position = currentCheckpoint.position;

            yield return new WaitForSeconds(1f);

            ActivateFirstPersonController(true);
        }
        else
        {
            yield return birdController.BirdDeath(currentCheckpoint.position);
        }

        isDead = false;

        if (fader != null)
        {
            yield return fader.FadeIn(.5f);
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
        capsuleCollider.enabled = shouldActivate;

        if (shouldActivate)
        {
            mainCam.transform.localPosition = camStartPos;
            mainCam.transform.localRotation = camStartRot;
        }
    }

    public void SetPlayerPhase(PlayerPhase newPhase)
    {
        currentPhase = newPhase;

        onPhaseChange(newPhase);
    }

    public void UpdateCheckpoint(Transform _checkpoint)
    {
        currentCheckpoint = _checkpoint;
    }

    public PlayerPhase GetPlayerPhase()
    {
        return currentPhase;
    }
}