using System;
using UnityEngine;

public class BirdTrigger : MonoBehaviour
{
    public event Action onDeath;

    private void OnTriggerEnter(Collider other)
    {
        Whale hitWhale = other.GetComponentInParent<Whale>();

        if(hitWhale != null)
        {
            onDeath();
        }
    }
}
