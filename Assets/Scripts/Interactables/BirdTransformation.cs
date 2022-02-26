using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdTransformation : MonoBehaviour
{
    [SerializeField] float timeToTransformation = 5f;
    [SerializeField] GameObject oobToTurnOff = null;

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

        yield return new WaitForSeconds(timeToTransformation);

        StartCoroutine(player.SetNewPhase(PlayerPhase.Two));

        whaleManager.ActivateWhalePhase();
    }
}
