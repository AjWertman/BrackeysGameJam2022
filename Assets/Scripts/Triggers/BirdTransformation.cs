using System.Collections;
using UnityEngine;

public class BirdTransformation : MonoBehaviour
{
    [SerializeField] Door doorParent = null;

    [SerializeField] AudioClip fallClip = null;
    [SerializeField] float timeToTransformation = 5f;
    [SerializeField] GameObject oobToTurnOff = null;
    [SerializeField] GameObject floorToOpen = null;
    [SerializeField] GameObject[] particles = null;

    SoundFXManager fXManager = null;
    WhaleManager whaleManager = null;
    PlayerController player = null;
    MusicPlayer musicPlayer = null;

    bool hasBegunTransformation = false;

    private void Awake()
    {
        fXManager = FindObjectOfType<SoundFXManager>();
        player = FindObjectOfType<PlayerController>();
        whaleManager = FindObjectOfType<WhaleManager>();
        musicPlayer = FindObjectOfType<MusicPlayer>();

        doorParent.onOpen += OpenTrapDoor;
    }

    public void OpenTrapDoor()
    {
        musicPlayer.Pause();
        foreach(GameObject particle in particles)
        {
            particle.SetActive(false);
        }

        oobToTurnOff.gameObject.SetActive(false);
        floorToOpen.gameObject.SetActive(false);

        fXManager.CreateSoundFX(fallClip, transform, .6f);
    }

    public IEnumerator BeginBirdTransformation()
    {
        if (hasBegunTransformation) yield break;
        hasBegunTransformation = true;

        player.SetPlayerPhase(PlayerPhase.Two);

        whaleManager.ActivateWhalePhase();
        gameObject.SetActive(false);
    }
}
