using System;
using UnityEngine;

public class BirdTrigger : MonoBehaviour
{
    public event Action onDeath;

    private void OnTriggerEnter(Collider other)
    {
        WhaleController hitWhale = other.GetComponentInParent<WhaleController>();

        if(hitWhale != null)
        {
            onDeath();
        }
    }
}
