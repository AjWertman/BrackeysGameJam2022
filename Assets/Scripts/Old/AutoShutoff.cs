using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShutoff : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(AutoShutoffObject(5));
    }

    private IEnumerator AutoShutoffObject(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
