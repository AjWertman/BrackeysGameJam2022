using System.Collections;
using UnityEngine;

public class WhaleSpawner : MonoBehaviour
{
    float minTimeBetweenSpawns = 4f;
    float maxTimeBetweenSpawns = 7f;

   
    float minYPos = -137f;
    float maxYPos = -60f;

    [SerializeField] bool canSpawn = false;

    public IEnumerator ActivateSpawner()
    {
        if (canSpawn) yield break;
        float timeToSpawn = Random.Range(0, 2);

        yield return new WaitForSeconds(timeToSpawn);

        canSpawn = true;
    }

    public IEnumerator SpawnWhale(Whale whale)
    {
        if (canSpawn)
        {
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
