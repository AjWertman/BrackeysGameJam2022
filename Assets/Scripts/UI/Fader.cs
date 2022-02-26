using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI faderText = null;

    CanvasGroup canvasGroup = null;
    Image image = null;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();

        faderText.text = "";
    }

    public IEnumerator FadeOut(float fadeOutTime, Color fadeColor, string _text)
    {
        image.color = fadeColor;

        faderText.color = GetMatchingTextColor(fadeColor);
        if (!string.IsNullOrEmpty(_text))
        {
            faderText.text = _text;
        }
        else
        {
            faderText.text = "";
        }

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeOutTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float fadeInTime)
    {
        faderText.text = "";
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeInTime;
            yield return null;
        }
    }

    public Color GetMatchingTextColor(Color faderImageColor)
    {
        if(faderImageColor == Color.black)
        {
            return Color.white;
        }
        else
        {
            return Color.black;
        }
    }
}
