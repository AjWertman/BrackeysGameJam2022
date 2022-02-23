using System;
using UnityEngine;

public class MorningTask : RaycastableObject
{
    [SerializeField] Texture2D objectTexture = null;
    [SerializeField] Material baseGlowMaterial = null;

    MeshRenderer[] meshRenderers = null;
    Material material = null;

    public event Action onTaskComplete;

    string fresnelPowerVariable = "_FresnelPower";

    bool isActivated = false;

    protected override void Awake()
    {
        base.Awake();
        gameObject.layer = 0;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        material = new Material(baseGlowMaterial);

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
        if (!isActivated) return;
        float fresnelPower = 1000;
        int layer = 0;

        if (shouldActivate)
        {
            fresnelPower = 2;
            layer = 6;
        }

        material.SetFloat("_FresnelPower", fresnelPower);
        gameObject.layer = layer;
        foreach(Transform child in transform)
        {
            child.gameObject.layer = layer;
        }
    }
}
