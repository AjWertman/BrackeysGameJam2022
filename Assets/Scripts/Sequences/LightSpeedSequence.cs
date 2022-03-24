using System.Collections;
using UnityEngine;

public class LightSpeedSequence : MonoBehaviour
{
    WhaleManager whaleManager = null;

    bool sequenceStarted = false;
    Fader fader;

    private void Awake()
    {
        whaleManager = FindObjectOfType<WhaleManager>();
        fader = FindObjectOfType<Fader>();
    }

    public IEnumerator ActivateLightSpeedSequence(Camera mainCamera)
    {

        //Activate light speed
        //Wait x seconds
        //transport to phase three
        //unactivate lightspeed

        if (!sequenceStarted)
        {
            sequenceStarted = true;
            yield return fader.FadeOut(2f,Color.black,null);

            whaleManager.TurnOffWhalePhase();

            StartCoroutine(fader.FadeIn(2f));
        }
    }
}
