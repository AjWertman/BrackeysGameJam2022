using System.Collections;
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

        //Activate light speed
        //Wait x seconds
        //transport to phase three
        //unactivate lightspeed

        if (!sequenceStarted)
        {
            sequenceStarted = true;
            yield return fader.FadeOut(2f,Color.black,null);

            StartCoroutine(fader.FadeIn(2f));
        }
    }
}
