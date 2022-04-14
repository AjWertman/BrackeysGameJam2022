using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    PlayerController playerController = null;
    BirdController birdController = null;

    CheckpointManager checkpointManager = null;
    MusicPlayer musicPlayer = null;
    Fader fader = null;
    MorningTasksSequence morningTasksSequence = null;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        birdController = playerController.GetComponent<BirdController>();

        checkpointManager = FindObjectOfType<CheckpointManager>();
        musicPlayer = FindObjectOfType<MusicPlayer>();
        fader = FindObjectOfType<Fader>();
        morningTasksSequence = FindObjectOfType<MorningTasksSequence>();

        playerController.onPhaseChange += ChangePhase;
    }

    private void ChangePhase(PlayerPhase newPhase)
    {
        StartCoroutine(HandlePhaseChanges(newPhase));
    }

    public IEnumerator HandlePhaseChanges(PlayerPhase newPhase)
    {
        if (newPhase == PlayerPhase.One)
        {
            birdController.Deactivate();
            playerController.ActivateFirstPersonController(true);
            morningTasksSequence.BeginMorningTasksSequence();
        }
        else if (newPhase == PlayerPhase.Two)
        {
            playerController.ActivateFirstPersonController(false);
            playerController.UpdateCheckpoint(checkpointManager.GetPhase2Checkpoint());

            birdController.Activate();
        }
        else if (newPhase == PlayerPhase.Three)
        {
            birdController.Deactivate();
            Transform phase3StartTransform = checkpointManager.GetPhase3StartTransform();
            playerController.transform.position = phase3StartTransform.position;
            playerController.transform.rotation = phase3StartTransform.rotation;

            playerController.UpdateCheckpoint(checkpointManager.GetPhase3Checkpoint());
  
            playerController.ActivateFirstPersonController(true);
        }

        musicPlayer.SetSong(newPhase);

        if (fader == null) yield break;
        yield return fader.FadeIn(1);
    }

}
