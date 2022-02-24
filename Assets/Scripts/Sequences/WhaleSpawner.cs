using System.Collections;
using UnityEngine;

public class WhaleSpawner : MonoBehaviour
{
    [SerializeField] float minTimeBetweenSpawns = 4f;
    [SerializeField] float maxTimeBetweenSpawns = 7f;

    //Test out
    float minYPos = 0;
    float maxYPos = 1000;

    bool canSpawn = true;

    public IEnumerator SpawnWhale(Whale whale)
    {
        if (!canSpawn) yield break;

        canSpawn = false;

        SetRandomYPosition();

        whale.transform.position = transform.position;
        whale.transform.rotation = transform.rotation;

        whale.gameObject.SetActive(true);

        whale.SetIsActive(true);

        float timeBetweenSpawns = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);

        yield return new WaitForSeconds(timeBetweenSpawns);

        canSpawn = true;
    }

    private void SetRandomYPosition()
    {
        Vector3 currentPos = transform.position;
        float yPos = Random.Range(minYPos, maxYPos);

        transform.position = new Vector3(currentPos.x, yPos, currentPos.z);
    }

    public bool CanSpawn()
    {
        return canSpawn;
    }
}
