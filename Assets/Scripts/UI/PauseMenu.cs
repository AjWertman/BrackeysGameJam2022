using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] Button resetLastCheckpointButton = null;
    [SerializeField] Button resetGameButton = null;
    [SerializeField] Button controlsButton = null;
    [SerializeField] Button quitButton = null;

    private void Awake()
    {
        resetLastCheckpointButton.onClick.AddListener(ResetToLastCheckpoint);
        resetGameButton.onClick.AddListener(() => StartCoroutine(ResetGame()));
        controlsButton.onClick.AddListener(ActivateControlsPage);
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    private void ResetToLastCheckpoint()
    {
        
    }

    private void ActivateControlsPage()
    {

    }

    private IEnumerator ResetGame()
    {
        yield return FindObjectOfType<Fader>().FadeOut(2, Color.white, null);
        yield return SceneManager.LoadSceneAsync(1);
        yield return FindObjectOfType<Fader>().FadeIn(1);
    }
}
