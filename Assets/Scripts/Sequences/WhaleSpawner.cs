using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Rename manager or something, then create whale spawner which works like the arrow spawner in shield wall
public class WhaleSpawner : MonoBehaviour
{
    [SerializeField] GameObject whalePrefab = null;
    [SerializeField] int numberOfWhalesInPool = 10;

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
        }
    }
}
