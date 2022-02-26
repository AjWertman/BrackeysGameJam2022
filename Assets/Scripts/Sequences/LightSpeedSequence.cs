using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpeedSequence : MonoBehaviour
{
    bool sequenceStarted = false;

    public IEnumerator ActivateLightSpeedSequence(Camera mainCamera)
    {
        if (!sequenceStarted)
        {
            sequenceStarted = true;
            //Do lightspeed stuff
            yield return null;
        }
    }
}
