using System;
using UnityEngine;

public class Door : RaycastableObject
{
    [SerializeField] GameObject objectToOpen = null;

    [SerializeField] bool isTrap = false;
    [SerializeField] bool isLocked = true;

    [SerializeField] bool isEndDoor = false;

    Animator animator = null;

    bool isOpen = false;
    //add to sound manager
    public event Action onOpen;

    protected override void Awake()
    {
        base.Awake();
        animator = objectToOpen.GetComponent<Animator>();
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

        //bool isOpen = animator.GetBool("open");
        //animator.SetBool("open", !isOpen);

        //Add sound fx so you dont need the isTrap or isEndDoor variable
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
