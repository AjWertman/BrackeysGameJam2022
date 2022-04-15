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
    List<WhaleController> whalePool = new List<WhaleController>();
    List<WhaleController> activeWhales = new List<WhaleController>();

    bool isActive = false;
    bool canHandleSpawner = false;
    bool canMakeSoundFX = true;

    SoundFXManager sfxManager = null;

    [SerializeField] AudioClip whaleSound0;

    private void Awake()
    {
        sfxManager = FindObjectOfType<SoundFXManager>();
        whaleSpawners = FindObjectsOfType<WhaleSpawner>();
    }

    private void Start()
    {
        CreateWhalePool();
    }

    private void Update()
    {
        if (!isActive) return;

        if (canMakeSoundFX)
        {
            StartCoroutine(HandleSFX());
        }

        if (!canHandleSpawner) return;

        StartCoroutine(HandleSpawners());
    }

    private IEnumerator HandleSFX()
    {
        if (!canMakeSoundFX) yield break;
        canMakeSoundFX = false;
        sfxManager.CreateSoundFX(whaleSound0, null, 1);
        yield return new WaitForSeconds(5f);
        canMakeSoundFX = true;
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

        foreach(WhaleController activeWhale in activeWhales)
        {
            activeWhale.gameObject.SetActive(false);
        }

        StopAllCoroutines();

        isActive = false;
        canMakeSoundFX = false;
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
            WhaleController whale = newWhaleInstance.GetComponent<WhaleController>();

            if (whalePool.Contains(whale)) return;
            whalePool.Add(whale);
            whale.gameObject.SetActive(false);
        }
    }

    public WhaleController GetRandomWhale()
    {
        WhaleController randomWhale = GetInactiveWhale();
        float randomScale = UnityEngine.Random.Range(whaleScaleMin, whaleScaleMax);

        Transform whaleTransform = randomWhale.transform;

        whaleTransform.localScale = new Vector3(randomScale, randomScale, randomScale);

        return randomWhale;
    }

    private WhaleController GetInactiveWhale()
    {
        WhaleController inactiveWhale = null;

        foreach (WhaleController whale in whalePool)
        {
            if (whale.IsActive()) continue;

            inactiveWhale = whale;
            break;
        }

        return inactiveWhale;
    }
}
