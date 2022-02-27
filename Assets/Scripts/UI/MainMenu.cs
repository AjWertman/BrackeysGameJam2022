using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject faderPrefab = null;
    [SerializeField] GameObject soundFXManagerPrefab = null;

    [SerializeField] Button startButton = null;
    [SerializeField] Button quitButton = null;

    [SerializeField] AudioClip alarmClock = null;

    Fader fader = null;
    SoundFXManager sfXManager = null;

    private void Awake()
    {
        startButton.onClick.AddListener(()=> StartCoroutine(StartGame()));
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    private void Start()
    {
        GameObject faderInstance = Instantiate(faderPrefab);
        fader = faderInstance.GetComponent<Fader>();
        fader.GetComponent<CanvasGroup>().alpha = 0;

        DontDestroyOnLoad(faderInstance);

        GameObject soundManagerInstance = Instantiate(soundFXManagerPrefab);
        sfXManager = soundManagerInstance.GetComponent<SoundFXManager>();

        DontDestroyOnLoad(faderInstance);
        DontDestroyOnLoad(sfXManager);
    }

    private IEnumerator StartGame()
    {
        yield return fader.FadeOut(2, Color.white, null);
        yield return SceneManager.LoadSceneAsync(1);
    }
}
