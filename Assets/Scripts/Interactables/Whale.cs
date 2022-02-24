using System;
using UnityEngine;

public class Whale : MonoBehaviour
{
    [SerializeField] float whaleSpeed = 5;

    bool isActive = false;

    private void Update()
    {
        if (isActive)
        {
            Swim();
        }
    }

    private void Swim()
    {
        transform.Translate(Vector3.forward * whaleSpeed * Time.deltaTime);
    }

    public void SetIsActive(bool _isActive)
    {
        isActive = _isActive;
    }

    public bool IsActive()
    {
        return isActive;
    }

    private void DeactivateWhale()
    {
        SetIsActive(false);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        //OutOfBounds oob = other.GetComponent<OutOfBounds>();   

        //if(oob != null)
        //{
        //    DeactivateWhale();
        //}
    }
}
