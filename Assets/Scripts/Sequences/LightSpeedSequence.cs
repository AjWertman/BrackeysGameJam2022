using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpeedSequence : MonoBehaviour
{
    bool sequenceStarted = false;

    public IEnumerator ActivateLightSpeedSequence(Camera camera)
    { 
        //Do lightspeed stuff

        print("hit light speed"); 
        yield return new WaitForSeconds(3);
    }
}
