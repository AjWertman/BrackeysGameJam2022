using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndDoor : MonoBehaviour
{
    [SerializeField] GameObject endScreen = null;
    [SerializeField] AudioClip alarmClock = null;

    SoundFXManager soundFXManager = null;
    EnemyController enemy;
    Door myDoor = null;
    Fader fader = null;

    private void Awake()
    {
        endScreen.SetActive(false);
        endScreen.GetComponentInChildren<Button>().onClick.AddListener(() => Application.Quit());
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
    }
}
