using UnityEngine;

public abstract class RaycastableObject : MonoBehaviour
{
    protected virtual void Awake()
    {
        gameObject.layer = 6;
    }

    public string activationText = "";
    public float activationDistance = 3f;
    public abstract void OnClick();
}
