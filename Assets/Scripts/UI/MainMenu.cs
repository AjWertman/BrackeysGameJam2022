using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button startButton = null;
    [SerializeField] Button quitButton = null;

    private void Awake()
    {
        startButton.onClick.AddListener(()=> StartCoroutine(StartGame()));
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    //replace find fader to fader that persists
    private IEnumerator StartGame()
    {
        yield return FindObjectOfType<Fader>().FadeOut(2, Color.white, null);
        yield return SceneManager.LoadSceneAsync(1);
        //Play alarm clock
        yield return FindObjectOfType<Fader>().FadeIn(1);
        //Start scene 1
        //activate controls page
    }
}
