using System;
using UnityEngine;

public class Door : RaycastableObject
{
    [SerializeField] GameObject objectToOpen = null;

    [SerializeField] bool isTrap = false;
    [SerializeField] bool isLocked = true;

    Animator animator = null;

    bool isOpen = false;

    //add to sound manager
    public event Action onOpen;

    protected override void Awake()
    {
        base.Awake();
        animator = objectToOpen.GetComponent<Animator>();
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

        objectToOpen.gameObject.SetActive(shouldSetActive);

        //bool isOpen = animator.GetBool("open");
        //animator.SetBool("open", !isOpen);


        //Add sound fx so you dont need the isTrap variable
        if (!isTrap) return;
        onOpen();
    }

    public void LockDoor(bool shouldLock)
    {
        isLocked = shouldLock;
    }
}
