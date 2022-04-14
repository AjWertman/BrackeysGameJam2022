using System;
using UnityEngine;

public class Door : RaycastableObject
{
    [SerializeField] AudioClip doorSound = null;

    [SerializeField] bool isMoreThanASimpleDoor = false;
    [SerializeField] bool isLocked = true;

    Animator animator = null;
    SoundFXManager soundFXManager = null;

    public event Action onOpen;
    bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInParent<Animator>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
    }

    public void Update()
    {
        if (isOpen)
        {
            activationText = "";
            return;
        }

        if (isLocked && activationText != "Door is locked")
        {
            activationText = "Door is locked";
        }
        else if(!isLocked && activationText != "Open the door")
        {
            activationText = "Open the door";
        }
    }

    public override void OnClick()
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        if (isLocked == true) return;

        if (isMoreThanASimpleDoor)
        {
            activationText = "";
            onOpen();
        }
        else
        {
            isOpen = true;
            animator.SetTrigger("open");
            soundFXManager.CreateSoundFX(doorSound, transform, .1f);
        }
    }

    public void CloseDoor()
    {
        animator.SetTrigger("close");
    }

    public void LockDoor(bool shouldLock)
    {
        isLocked = shouldLock;
    } 
}
