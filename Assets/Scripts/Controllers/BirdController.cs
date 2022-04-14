using System;
using System.Collections;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] GameObject birdObject = null;
    [SerializeField] Transform birdCamTransform = null;
    [SerializeField] AudioSource wingFlapSource = null;
    [SerializeField] AudioClip deathSound = null;

    [SerializeField] float flySpeed = 20f;
    [SerializeField] float pitchFactor = -2f;
    [SerializeField] float yawFactor = -20f;
    [SerializeField] float rollFactor = -3f;

    Camera mainCam = null;
    CharacterController characterController = null;
    SoundFXManager soundFXManager = null;

    Vector3 moveDirection = Vector3.zero;

    bool canFly = false;
    bool isTransitioningCam = false;

    public event Action onDeath;

    private void Awake()
    {
        mainCam = Camera.main;

        characterController = GetComponent<CharacterController>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
    }

    private void Update()
    {
        if (isTransitioningCam)
        {
            mainCam.transform.position = Vector3.MoveTowards(mainCam.transform.position, birdCamTransform.position, 10 * Time.deltaTime);

            mainCam.transform.eulerAngles = Vector3.Slerp(mainCam.transform.eulerAngles, birdCamTransform.eulerAngles, 100 * Time.deltaTime);

            //mainCam.transform.eulerAngles = Vector3.MoveTowards(mainCam.transform.eulerAngles, birdCamTransform.eulerAngles, 100 * Time.deltaTime);

            if (mainCam.transform.position == birdCamTransform.position && mainCam.transform.eulerAngles == birdCamTransform.eulerAngles)
            {
                isTransitioningCam = false;
            }
        }

        if (!canFly) return;
        FlyForward();
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

        moveDirection = new Vector3(horizontalInput, verticalInput, 1).normalized;
        characterController.Move(moveDirection * flySpeed * Time.deltaTime);

        float pitchDirection = moveDirection.y * pitchFactor;
        float yawDirection = moveDirection.x * yawFactor;
        float rollDirection = moveDirection.x * rollFactor;

        float pitch = pitchDirection;
        float yaw = yawDirection;
        float roll = rollDirection;

        birdObject.transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
    }

    public IEnumerator BirdDeath(Vector3 newPosition)
    {
        soundFXManager.CreateSoundFX(deathSound, transform, 1);

        characterController.enabled = false;

        transform.position = newPosition;
        Vector3 lookPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        transform.LookAt(lookPosition);

        yield return new WaitForSeconds(1f);

        characterController.enabled = true;
    }

    public void Activate()
    {
        isTransitioningCam = true;
        characterController.enabled = true;
        birdObject.SetActive(true);
        canFly = true;

        StartFlying();
        StartCoroutine(BeginTransitionSafety());
    }

    private IEnumerator BeginTransitionSafety()
    {
        yield return new WaitForSeconds(.75f);

        bool isTransitioned = mainCam.transform.position == birdCamTransform.position && mainCam.transform.eulerAngles == birdCamTransform.eulerAngles;

        if (!isTransitioned)
        {
            mainCam.transform.position = birdCamTransform.position;
            mainCam.transform.eulerAngles = birdCamTransform.eulerAngles;
            isTransitioningCam = false;
        }
        else yield break;
    }

    public void Deactivate()
    {
        characterController.enabled = false;
        birdObject.SetActive(false);
        canFly = false;
    }
}
