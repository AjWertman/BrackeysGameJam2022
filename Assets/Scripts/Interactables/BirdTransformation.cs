using System.Collections;
using UnityEngine;

public class BirdTransformation : MonoBehaviour
{
    [SerializeField] float timeToTransformation = 5f;
    [SerializeField] GameObject oobToTurnOff = null;
    [SerializeField] GameObject floorToOpen = null;

    WhaleManager whaleManager = null;
    Door doorParent = null;
    PlayerController player = null;
    MusicPlayer musicPlayer = null;

    bool hasBegunTransformation = false;

    private void Awake()
    {
        doorParent = GetComponent<Door>();
        player = FindObjectOfType<PlayerController>();
        whaleManager = FindObjectOfType<WhaleManager>();
        musicPlayer = FindObjectOfType<MusicPlayer>();
        doorParent.onOpen += () => StartCoroutine(BeginBirdTransformation());
    }

    public IEnumerator BeginBirdTransformation()
    {
        if (hasBegunTransformation) yield break;
        hasBegunTransformation = true;

        musicPlayer.Pause();

        oobToTurnOff.gameObject.SetActive(false);
        floorToOpen.gameObject.SetActive(false);

        yield return new WaitForSeconds(timeToTransformation);

        player.SetPlayerPhase(PlayerPhase.Two);

        whaleManager.ActivateWhalePhase();
    }
}
