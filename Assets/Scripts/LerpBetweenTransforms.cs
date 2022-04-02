using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpBetweenTransforms : MonoBehaviour
{
    public void LerpTransforms(Transform transformToLerp)
    {
        Transform myTransform = transform;

        LerpPositions(myTransform, transformToLerp);
    }

    private void LerpPositions(Transform myTransform, Transform transformToLerp)
    {
        Vector3 myPosition = myTransform.position;
        Vector3 positionToLerp = transformToLerp.position;
    }
}
