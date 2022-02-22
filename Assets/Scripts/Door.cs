using System;
using UnityEngine;

public class Door : RaycastableObject
{
    [SerializeField] GameObject objectToOpen = null;

    [SerializeField] bool isTrap = false;

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
        objectToOpen.gameObject.SetActive(false);

        //bool isOpen = animator.GetBool("open");
        //animator.SetBool("open", !isOpen);

        if (!isTrap) return;
        onOpen();
    }
}
