using TMPro;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI activationText = null;

    private void Start()
    {
        DeactivateActivationText();
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
}
