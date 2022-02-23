using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] Camera birdCamera = null;
    [SerializeField] float flySpeed = 20f;
    [SerializeField] float turnSpeed = 20f;

    CharacterController characterController = null;
    Vector3 moveDirection = Vector3.zero;

    bool canFly = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!canFly) return;
        Fly();
    }

    private void Fly()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 1).normalized;

        characterController.Move(moveDirection * flySpeed * Time.deltaTime);
    }

    public void StartFlying()
    {
        canFly = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        LightSpeedSequence lightSpeedSequence = other.GetComponent<LightSpeedSequence>();
        if (lightSpeedSequence == null) return;

        lightSpeedSequence.ActivateLightSpeedSequence(birdCamera);
    }
}
