using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolBooks : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToTurnOff = null;

    private void Awake()
    {
        GetComponent<MorningTask>().onTaskComplete += GrabSchoolBooks;
    }

    private void GrabSchoolBooks()
    {
        foreach(GameObject book in objectsToTurnOff)
        {
            book.SetActive(false);
        }

        //Play sound??
    }
}
