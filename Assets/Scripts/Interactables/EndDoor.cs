using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndDoor : MonoBehaviour
{
    [SerializeField] GameObject endScreen = null;
    [SerializeField] Button mainMenuButton = null;
    [SerializeField] Button quitGameButton = null;
    [SerializeField] AudioClip alarmClock = null;

    SoundFXManager soundFXManager = null;
    EnemyController enemy;
    Door myDoor = null;
    Fader fader = null;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene(0));
        quitGameButton.onClick.AddListener(() => Application.Quit());
        endScreen.SetActive(false);

        enemy = FindObjectOfType<EnemyController>();
        myDoor = GetComponent<Door>();
        fader = FindObjectOfType<Fader>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
        myDoor.onOpen += ()=> StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        enemy.SetIsActivated(false);
        soundFXManager.CreateSoundFX(alarmClock, null);
        yield return fader.FadeOut(2, Color.white, "The End!");
        yield return new WaitForSeconds(2f);

        fader.GetComponent<CanvasGroup>().alpha = 0;
        endScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
