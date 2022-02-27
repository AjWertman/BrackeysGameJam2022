using System;
using UnityEngine;

public class Door : RaycastableObject
{
    [SerializeField] GameObject objectToOpen = null;

    [SerializeField] AudioClip doorSound = null;

    [SerializeField] bool isTrap = false;
    [SerializeField] bool isLocked = true;

    [SerializeField] bool isEndDoor = false;

    Animator animator = null;
    SoundFXManager soundFXManager = null;

    bool isOpen = false;
    //add to sound manager
    public event Action onOpen;

    protected override void Awake()
    {
        base.Awake();
        animator = objectToOpen.GetComponent<Animator>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
    }

    public void Update()
    {
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
        OpenDoor(!isOpen);
    }

    public void OpenDoor(bool shouldOpen)
    {
        if (isLocked == true) return;
        
        //Switch to animation insteadof activating
        bool shouldSetActive = !shouldOpen;

        if (!isEndDoor)
        {
            objectToOpen.gameObject.SetActive(shouldSetActive);
        }

        if(doorSound!= null)
        {
            soundFXManager.CreateSoundFX(doorSound, transform);
        }

        if (isTrap || isEndDoor)
        {
            onOpen();
        }
    }

    public void LockDoor(bool shouldLock)
    {
        isLocked = shouldLock;
    }
}
