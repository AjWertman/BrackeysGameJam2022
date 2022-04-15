using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    [SerializeField] GameObject controlsPage = null;
    [SerializeField] Button controlsButton = null;
    [SerializeField] AudioClip uiSound = null;

    [SerializeField] TextMeshProUGUI activationText = null;

    SoundFXManager soundFXManager = null;

    public event Action onControlsClose;

    private void Awake()
    {
        DeactivateActivationText();
        soundFXManager = FindObjectOfType<SoundFXManager>();

        controlsButton.onClick.AddListener(() => DeactivateControls());
        controlsButton.onClick.AddListener(() => soundFXManager.CreateSoundFX(uiSound, Camera.main.transform, 1));
    }

    public void ActivateActivationText(string activationString)
    {
        activationText.text = activationString;

        if (activationText.gameObject.activeSelf) return;
        activationText.gameObject.SetActive(true);
    }

    public void DeactivateActivationText()
    {
        if (!activationText.gameObject.activeSelf) return;

        activationText.gameObject.SetActive(false);
        activationText.text = "";
    }

    public void DeactivateControls()
    {
        controlsPage.SetActive(false);
        onControlsClose();
    }

    public void ActivateControls()
    {
        controlsPage.SetActive(true);
    }
}
