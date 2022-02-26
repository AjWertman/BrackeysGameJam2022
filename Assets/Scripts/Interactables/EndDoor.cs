using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDoor : MonoBehaviour
{
    EnemyController enemy;
    Door myDoor = null;
    Fader fader = null;

    private void Awake()
    {
        enemy = FindObjectOfType<EnemyController>();
        myDoor = GetComponent<Door>();
        fader = FindObjectOfType<Fader>();
        myDoor.onOpen += ()=> StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        enemy.SetIsActivated(false);
        yield return fader.FadeOut(2, Color.white, "The End!");

        yield return new WaitForSeconds(3f);
        Application.Quit();
    }
}
