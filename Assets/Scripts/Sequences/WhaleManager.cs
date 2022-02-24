using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rename manager or something, then create whale spawner which works like the arrow spawner in shield wall
public class WhaleManager : MonoBehaviour
{
    [SerializeField] GameObject whalePrefab = null;
    [SerializeField] int numberOfWhalesInPool = 10;

    [SerializeField] float whaleScaleMin = 1;
    [SerializeField] float whaleScaleMax = 5;

    List<Whale> whalePool = new List<Whale>();

    private void Start()
    {
        CreateWhalePool();
    }

    private void CreateWhalePool()
    {
        for (int i = 0; i < numberOfWhalesInPool; i++)
        {
            GameObject newWhaleInstance = Instantiate(whalePrefab, transform);
            Whale whale = newWhaleInstance.GetComponent<Whale>();

            if (whalePool.Contains(whale)) return;
            whalePool.Add(whale);
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
