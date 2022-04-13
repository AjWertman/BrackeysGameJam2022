using UnityEngine;

public class SchoolBooks : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToTurnOff = null;
    [SerializeField] AudioClip pickupClip = null;

    SoundFXManager soundFXManager = null;

    private void Awake()
    {
        GetComponent<MorningTask>().onTaskComplete += GrabSchoolBooks;

        soundFXManager = FindObjectOfType<SoundFXManager>();
    }

    private void GrabSchoolBooks()
    {
        foreach(GameObject book in objectsToTurnOff)
        {
            book.SetActive(false);
        }

        soundFXManager.CreateSoundFX(pickupClip, transform, .75f);
    }
}
