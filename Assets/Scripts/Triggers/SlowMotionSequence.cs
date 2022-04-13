using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SlowMotionSequence : MonoBehaviour
{
    [SerializeField] Transform startTransform = null;
    [SerializeField] Transform endTransform = null;

    PlayerController player = null;
    RigidbodyFirstPersonController fpController = null;
    EnemyController enemy = null;

    Vector3 startPosition = Vector3.zero;
    Vector3 endPosition = Vector3.zero;

    float maxDistanceBetweenPoints = 0f;

    Transform playerTransform = null;

    bool isActivated = false;

    [SerializeField] float minTimeScale = .4f;
    [SerializeField] float sensitivityDropoffScale = .4f;
    float xSensitivity = 0;
    float ySensitivity = 0;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        enemy = FindObjectOfType<EnemyController>();

        fpController = player.GetComponent<RigidbodyFirstPersonController>();
        playerTransform = player.transform;
        xSensitivity = fpController.mouseLook.XSensitivity;
        ySensitivity = fpController.mouseLook.YSensitivity;

        startPosition = startTransform.position;
        endPosition = endTransform.position;

        maxDistanceBetweenPoints = Vector3.Distance(startPosition, endPosition);
    }

    private void Update()
    {
        if (isActivated)
        {
            Time.timeScale = Mathf.Lerp(1, minTimeScale, GetDistancePercentage());

            fpController.mouseLook.XSensitivity = Mathf.Lerp(xSensitivity, sensitivityDropoffScale, GetDistancePercentage());
            fpController.mouseLook.YSensitivity = Mathf.Lerp(ySensitivity, sensitivityDropoffScale, GetDistancePercentage());
        }
    }

    private float GetDistancePercentage()
    {
        float distanceFromStart = GetDistancePoint(startPosition);

        return distanceFromStart/maxDistanceBetweenPoints;
    }

    private float GetDistancePoint(Vector3 pointToTest)
    {
        Vector3 playerPosition = playerTransform.position;
        Vector3 pointWithYOffset = new Vector3(pointToTest.x, playerPosition.y, pointToTest.z);

        return Vector3.Distance(playerPosition, pointWithYOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        isActivated = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActivated) return;
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        Time.timeScale = 1;
        isActivated = false;
    }
}
