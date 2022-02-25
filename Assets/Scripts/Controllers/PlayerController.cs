using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerPhase { One, Two, Three }

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject firstPersonCamObject = null;
    [SerializeField] GameObject birdControllerObject = null;
    [SerializeField] Transform phase3StartTransform = null;

    [SerializeField] PlayerPhase phase = PlayerPhase.One;

    Camera firstPersonCamera = null;
    RigidbodyFirstPersonController firstPersonController = null;
    Rigidbody rb = null;
    UICanvas uiCanvas = null;

    CheckpointManager checkpointManager = null;
    RaycastableObject currentRaycastableObject = null;

    bool isDead = false;

    private void Awake()
    {
        firstPersonCamera = firstPersonCamObject.GetComponent<Camera>();
        firstPersonController = GetComponent<RigidbodyFirstPersonController>();
        rb = GetComponent<Rigidbody>();
        uiCanvas = FindObjectOfType<UICanvas>();

        checkpointManager = FindObjectOfType<CheckpointManager>();
    }

    private void Start()
    {
        ActivateCursor(false);
        SetNewPhase(PlayerPhase.One);
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
            //uiCanvas.ActivatePauseMenu();

            ActivateCursor(true);
        }

        HandleRaycasts();
    }

    public void SetPlayerPhase(PlayerPhase newPhase)
    {
        phase = newPhase;
    }

    private void HandleRaycasts()
    {
        Ray ray = firstPersonCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("RaycastableObject")))
        {
            RaycastableObject raycast = hit.collider.GetComponent<RaycastableObject>();
            currentRaycastableObject = raycast;
        }     

        if (currentRaycastableObject != null)
        {
            bool isInRange = (Vector3.Distance(transform.position, currentRaycastableObject.transform.position) < currentRaycastableObject.activationDistance);

            if (isInRange)
            {
                uiCanvas.ActivateActivationText(currentRaycastableObject.activationText);
            }
            else
            {
                uiCanvas.DeactivateActivationText();
            }          
        }
        else
        {
            uiCanvas.DeactivateActivationText();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            currentRaycastableObject.OnClick();
        }
    }

    public void SetNewPhase(PlayerPhase _phase)
    {
        phase = _phase;

        if (phase == PlayerPhase.One)
        {
            birdControllerObject.SetActive(false);
            ActivateFirstPersonController(true);
        }
        else if (phase == PlayerPhase.Two)
        {
            ActivateFirstPersonController(false);
            birdControllerObject.SetActive(true);
            birdControllerObject.GetComponent<BirdController>().StartFlying();

            //transform.LookAt(Vector3.forward);
        }
        else if (phase == PlayerPhase.Three)
        {           
            birdControllerObject.SetActive(false);

            transform.position = phase3StartTransform.position;
            transform.rotation = phase3StartTransform.rotation;

            ActivateFirstPersonController(true);
        }
    }

    public void ActivateFirstPersonController(bool shouldActivate)
    {
        firstPersonController.enabled = shouldActivate;
        rb.useGravity = shouldActivate;
        rb.isKinematic = !shouldActivate;
        firstPersonCamObject.SetActive(shouldActivate);
    }

    public void Die()
    {
        if (isDead) return;

        //FadeOut

        isDead = true;
        ActivateFirstPersonController(false);
        Transform checkpointTransform = checkpointManager.GetCheckpointPosition(phase);

        transform.position = checkpointTransform.position;
        transform.rotation = checkpointTransform.rotation;

        ActivateFirstPersonController(true);
        isDead = false;

        //FadeIn
    }

    private void OnTriggerEnter(Collider other)
    {
        LightSpeedSequence lightSpeedSequence = other.GetComponent<LightSpeedSequence>();

        if(lightSpeedSequence != null)
        {
            StartCoroutine(Phase3Transition(lightSpeedSequence));
        }

        SchoolChaseSequence schoolChaseSequence = other.GetComponent<SchoolChaseSequence>();

        if(schoolChaseSequence != null)
        {
            schoolChaseSequence.BeginChaseSequence();
            return;
        }

        Whale hitWhale = other.GetComponent<Whale>();
        OutOfBounds oob = other.GetComponent<OutOfBounds>();

        if (hitWhale != null || oob != null)
        {
            Die();
        }              
    }

    private IEnumerator Phase3Transition(LightSpeedSequence lightSpeedSequence)
    {
        yield return lightSpeedSequence.ActivateLightSpeedSequence(firstPersonCamera);

        SetNewPhase(PlayerPhase.Three);
    }
}
