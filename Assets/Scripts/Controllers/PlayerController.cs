using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerState { FirstPerson, Bird}

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject firstPersonCamObject = null;
    [SerializeField] GameObject birdControllerObject = null;

    Camera firstPersonCamera = null;
    RigidbodyFirstPersonController firstPersonController = null;
    Rigidbody rb = null;
    UICanvas uiCanvas = null;

    RaycastableObject currentRaycastableObject = null;

    private void Awake()
    {
        firstPersonCamera = firstPersonCamObject.GetComponent<Camera>();
        firstPersonController = GetComponent<RigidbodyFirstPersonController>();
        rb = GetComponent<Rigidbody>();
        uiCanvas = FindObjectOfType<UICanvas>();
    }

    private void Start()
    {
        ActivateCursor(false);
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

    private void HandleRaycasts()
    {
        Ray ray = firstPersonCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("RaycastableObject")))
        {
            RaycastableObject raycast = hit.collider.GetComponent<RaycastableObject>();
            currentRaycastableObject = raycast;
            print(hit.collider.name);
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

    public void TransformPlayer(PlayerState playerState)
    {
        if(playerState == PlayerState.FirstPerson)
        {
            birdControllerObject.SetActive(false);
            ActivateFirstPersonController(true);
        }
        else if(playerState == PlayerState.Bird)
        {
            ActivateFirstPersonController(false);
            birdControllerObject.SetActive(true);
            birdControllerObject.GetComponent<BirdController>().StartFlying();
        }
    }

    public void ActivateFirstPersonController(bool shouldActivate)
    {
        firstPersonController.enabled = shouldActivate;
        rb.useGravity = shouldActivate;
        rb.isKinematic = !shouldActivate;
        firstPersonCamObject.SetActive(shouldActivate);
    }

    private void OnTriggerEnter(Collider other)
    {
        Whale hitWhale = other.GetComponent<Whale>();
        if (hitWhale != null)
        {
            //Screen fx
            //Reset to checkpoint
        }
    }
}
