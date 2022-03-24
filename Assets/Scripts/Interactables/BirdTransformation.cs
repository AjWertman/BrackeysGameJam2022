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

    bool hasBegunTransformation = false;

    private void Awake()
    {
        doorParent = GetComponent<Door>();
        player = FindObjectOfType<PlayerController>();
        whaleManager = FindObjectOfType<WhaleManager>();
        doorParent.onOpen += () => StartCoroutine(BeginBirdTransformation());
    }

    public IEnumerator BeginBirdTransformation()
    {
        if (hasBegunTransformation) yield break;
        hasBegunTransformation = true;

        oobToTurnOff.gameObject.SetActive(false);
        floorToOpen.gameObject.SetActive(false);

        yield return new WaitForSeconds(timeToTransformation);

        player.SetPlayerPhase(PlayerPhase.Two);

        whaleManager.ActivateWhalePhase();
    }
}
