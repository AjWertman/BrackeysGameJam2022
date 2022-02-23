using System;
using UnityEngine;

public class Door : RaycastableObject
{
    [SerializeField] GameObject objectToOpen = null;

    [SerializeField] bool isTrap = false;
    [SerializeField] bool isLocked = true;

    Animator animator = null;

    //add to sound manager
    public event Action onOpen;

    protected override void Awake()
    {
        base.Awake();
        animator = objectToOpen.GetComponent<Animator>();
    }

    public override void OnClick()
    {
        if (isLocked == true) return;

        objectToOpen.gameObject.SetActive(false);

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
