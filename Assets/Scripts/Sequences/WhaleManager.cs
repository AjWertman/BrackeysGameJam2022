using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleManager : MonoBehaviour
{
    [SerializeField] GameObject whalePrefab = null;
    [SerializeField] int numberOfWhalesInPool = 10;

    [SerializeField] float whaleScaleMin = 1;
    [SerializeField] float whaleScaleMax = 5;

    WhaleSpawner[] whaleSpawners = null;
    List<Whale> whalePool = new List<Whale>();

    bool isActive = false;
    bool canHandleSpawner = false;

    private void Awake()
    {
        whaleSpawners = FindObjectsOfType<WhaleSpawner>();
    }

    private void Start()
    {
        CreateWhalePool();
    }

    private void Update()
    {
        if (!isActive) return;
        if (!canHandleSpawner) return;

        StartCoroutine(HandleSpawners());
    }

    public void ActivateWhalePhase()
    {
        foreach (WhaleSpawner spawner in whaleSpawners)
        {
            StartCoroutine(spawner.ActivateSpawner());
        }

        isActive = true;
        canHandleSpawner = true;
    }

    public void TurnOffWhalePhase()
    {
        foreach (WhaleSpawner spawner in whaleSpawners)
        {
            spawner.gameObject.SetActive(false);
        }

        isActive = false;
    }

    private IEnumerator HandleSpawners()
    {
        if (canHandleSpawner)
        {
            canHandleSpawner = false;

            foreach (WhaleSpawner spawner in whaleSpawners)
            {
                if (spawner.CanSpawn())
                {
                    StartCoroutine(spawner.SpawnWhale(GetRandomWhale()));
                }
            }
            yield return new WaitForSeconds(1f);

            canHandleSpawner = true;
        }
    }

    private void CreateWhalePool()
    {
        for (int i = 0; i < numberOfWhalesInPool; i++)
        {
            GameObject newWhaleInstance = Instantiate(whalePrefab, transform);
            Whale whale = newWhaleInstance.GetComponent<Whale>();

            if (whalePool.Contains(whale)) return;
            whalePool.Add(whale);
            whale.gameObject.SetActive(false);
        }
    }

    public Whale GetRandomWhale()
    {
        Whale randomWhale = GetInactiveWhale();
        float randomScale = UnityEngine.Random.Range(whaleScaleMin, whaleScaleMax);

        Transform whaleTransform = randomWhale.transform;

        whaleTransform.localScale = new Vector3(randomScale, randomScale, randomScale);

        return randomWhale;
    }

    private Whale GetInactiveWhale()
    {
        Whale inactiveWhale = null;

        foreach (Whale whale in whalePool)
        {
            if (whale.IsActive()) continue;

            inactiveWhale = whale;
            break;
        }

        return inactiveWhale;
    }
}
