using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpeedSequence : MonoBehaviour
{
    bool sequenceStarted = false;
    Fader fader;

    private void Awake()
    {
        fader = FindObjectOfType<Fader>();
    }

    public IEnumerator ActivateLightSpeedSequence(Camera mainCamera)
    {
        if (!sequenceStarted)
        {
            sequenceStarted = true;
            yield return fader.FadeOut(2f,Color.black,null);

            StartCoroutine(fader.FadeIn(2f));
        }
    }
}
