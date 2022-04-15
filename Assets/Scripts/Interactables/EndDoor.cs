using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndDoor : MonoBehaviour
{
    [SerializeField] AudioClip alarmClock = null;

    SlowMotionSequence slowMotionSequence = null;
    SoundFXManager soundFXManager = null;
    EnemyController enemy;
    Door myDoor = null;
    Fader fader = null;

    bool isEnding = false;

    private void Awake()
    {
        slowMotionSequence = FindObjectOfType<SlowMotionSequence>();
        enemy = FindObjectOfType<EnemyController>();
        myDoor = GetComponent<Door>();
        fader = FindObjectOfType<Fader>();
        soundFXManager = FindObjectOfType<SoundFXManager>();
        myDoor.onOpen += ()=> StartCoroutine(EndGame());
    }

    public IEnumerator EndGame()
    {
        if (isEnding) yield break;
        isEnding = true;

        slowMotionSequence.enabled = false;
        Time.timeScale = 1;

        enemy.SetIsActivated(false);
        soundFXManager.CreateSoundFX(alarmClock, null, .75f);

        yield return fader.FadeOut(2, Color.white, "The End!");
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(2);
    }
}
