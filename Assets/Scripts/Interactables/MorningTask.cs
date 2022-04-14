using System;
using UnityEngine;

public class MorningTask : RaycastableObject
{
    [SerializeField] Texture2D objectTexture = null;
    [SerializeField] Material baseGlowMaterial = null;
    [SerializeField] string activeActivationText = null;

    MeshRenderer[] meshRenderers = null;
    Material material = null;

    public event Action onTaskComplete;

    string fresnelPowerVariable = "_FresnelPower";

    [SerializeField] bool isActivated = false;

    protected override void Awake()
    {
        base.Awake();
        gameObject.layer = 0;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        material = new Material(baseGlowMaterial);

        material.SetTexture("_Texture", objectTexture);

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.material = material;
        }
    }

    public override void OnClick()
    {
        if (isActivated == false) return;
        onTaskComplete();
    }

    public void HighlightTask(bool shouldActivate)
    {
        float boolIsActiveFloat = 0f;

        int layer = 0;

        if (shouldActivate)
        {
            boolIsActiveFloat = 1f;
            layer = 6;
        }

        material.SetFloat("_isActive", boolIsActiveFloat);
        gameObject.layer = layer;
        foreach(Transform child in transform)
        {
            child.gameObject.layer = layer;
        }
    }

    public void ActivateTask(bool shouldActivate)
    {
        isActivated = shouldActivate;

        if (shouldActivate)
        {
            activationText = activeActivationText;
        }
        else
        {
            activationText = "";
        }

        HighlightTask(shouldActivate);
    }
}
