using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdTransformation : MonoBehaviour
{
    [SerializeField] float timeToTransformation = 5f;
    Door doorParent = null;
    PlayerController player = null;

    bool hasBegunTransformation = false;

    private void Awake()
    {
        doorParent = GetComponent<Door>();
        player = FindObjectOfType<PlayerController>();
        doorParent.onOpen += () => StartCoroutine(BeginBirdTransformation());
    }

    public IEnumerator BeginBirdTransformation()
    {
        if (hasBegunTransformation) yield break;
        hasBegunTransformation = true;

        yield return new WaitForSeconds(timeToTransformation);

        player.SetNewPhase(PlayerPhase.Two);
    }
}
