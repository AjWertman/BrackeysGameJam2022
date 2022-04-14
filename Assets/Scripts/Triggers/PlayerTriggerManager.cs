using UnityEngine;

public class PlayerTriggerManager : MonoBehaviour
{
    PlayerController playerController = null;
    Rigidbody rb = null;
    HumanSoundMaker humanSoundMaker = null;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        humanSoundMaker = GetComponent<HumanSoundMaker>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerPhase currentPhase = playerController.GetPlayerPhase();

        if (currentPhase == PlayerPhase.One)
        {
            BirdTransformation birdTransformation = other.GetComponent<BirdTransformation>();

            if (birdTransformation != null)
            {
                rb.isKinematic = true;
                StartCoroutine(birdTransformation.BeginBirdTransformation());
            }
        }
        else if (currentPhase == PlayerPhase.Two)
        {
            LightSpeedSequence lightSpeedSequence = other.GetComponent<LightSpeedSequence>();

            if (lightSpeedSequence != null)
            {
                StartCoroutine(lightSpeedSequence.ActivateLightSpeedSequence(playerController));
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

        if (humanSoundsTrigger != null)
        {
            humanSoundMaker.SetNewSounds(humanSoundsTrigger.GetHumanSounds());
        }

        OutOfBounds oob = other.GetComponent<OutOfBounds>();

        if (oob != null)
        {
            StartCoroutine(playerController.Die());
            return;
        }

        EndDoor endDoor = other.GetComponent<EndDoor>();

        if (endDoor != null)
        {
            StartCoroutine(endDoor.EndGame());
        }
    }
}
