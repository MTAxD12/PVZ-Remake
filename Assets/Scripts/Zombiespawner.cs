﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> zombieTypes = new List<GameObject>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [SerializeField] private List<float> spawnWeights = new List<float>(); // Ponderile sunt acum de tip float
    [SerializeField] private float adjustDifficultyBy = 20f; // Valoare de ajustare a dificultății

    [SerializeField] Transform flagParent;
    [SerializeField] GameObject flagPrefab;
    [SerializeField] Slider levelProgression;
    [SerializeField] private List<float> zombiePercentPerBW = new List<float>();
    [SerializeField] private float wavePercent = 13f;
    [SerializeField] private float betweenWavePerc = 0f;
    public float levelPercent = 0f;
    
    public int currentWave = 0;
    public int wavesCount = 2;
    public int[] zombiesPerWave;
    public bool activeWave = false;

    public int[] zombiesInBetweenWaves;
    public int zombiesSpawnedInBetweenWaves;

    public int zombiesSpawned = 0;
    public int zombiesKilledCurrent = 0;
    public int zombiesKilledTotal = 0;
    public int zombiesMax = 30;

    public float zombieCooldownSpawn = 20f;
    public float zombieRemainingCooldown = 0f;
    public bool isCooldown = true;

    public bool isWon = false;

    private Gamemanager gameManager;

    private void Start()
    {
        //progress bar
        levelProgression.maxValue = 100f;
        for (int i = 0; i < wavesCount; i++)
            Instantiate(flagPrefab, flagParent.position, Quaternion.identity, flagParent);

        wavePercent = 13f;
        betweenWavePerc = (100 - (wavesCount * wavePercent))/wavesCount;
      
        for (int i = 0; i < wavesCount; i++)
        {
            zombiePercentPerBW.Add(betweenWavePerc / zombiesInBetweenWaves[i]);
            Debug.Log(zombiePercentPerBW[i]);
        }

        //other
        gameManager = gameObject.GetComponent<Gamemanager>();
        zombieRemainingCooldown = 20f;
        zombiesMax = 0;
        for(int i = 0; i < zombiesPerWave.Length; i++) 
            zombiesMax += zombiesPerWave[i];

        for (int i = 0; i < zombiesInBetweenWaves.Length; i++)
            zombiesMax += zombiesInBetweenWaves[i];

        if (spawnWeights.Count != zombieTypes.Count)
        {
            Debug.LogError("Numărul de ponderi (`spawnWeights`) trebuie să fie egal cu numărul de tipuri de zombii (`zombieTypes`)!");
        }

    }

    private void Update()
    {
        if (zombiesKilledTotal < zombiesMax && !isWon)
        {
            if (zombiesSpawnedInBetweenWaves < zombiesInBetweenWaves[currentWave] && !activeWave)
            {
                if (isCooldown)
                {
                    zombieRemainingCooldown -= Time.deltaTime;
                    if (zombieRemainingCooldown <= 0)
                    {
                        isCooldown = false;
                        zombieRemainingCooldown = 0;
                        SpawnZombie();
                    }
                }
            }
            if (zombiesKilledCurrent == zombiesInBetweenWaves[currentWave] && !activeWave)
            {
                zombiesKilledCurrent = 0;
                activeWave = true;
                StartCoroutine(SpawnWave());
                AdjustSpawnWeights();

            }

            if (zombiesKilledCurrent == zombiesPerWave[currentWave] + 1 && activeWave)
            {
                activeWave = false;
                currentWave++;
                zombiesKilledCurrent = 0;
                AdjustSpawnWeights();

            }
        }
        else if (zombiesSpawned == zombiesKilledTotal && !isWon)
        {
            isWon = true;
            gameManager.WinGame();
        }
    }

    private void SpawnZombie()
    {
        zombiesSpawned++;
        zombiesSpawnedInBetweenWaves++;
        isCooldown = true;
        AddSliderValue(zombiePercentPerBW[currentWave]);

        if (zombiesSpawned == 1)
            zombieCooldownSpawn = 15f;
        if (zombiesSpawned == 4)
            zombieCooldownSpawn = 12.5f;
        zombieRemainingCooldown = zombieCooldownSpawn;

        int lane = Random.Range(0, spawnPoints.Count);
        GameObject zombieToSpawn = GetRandomZombieType();
        Instantiate(zombieToSpawn, spawnPoints[lane].position, Quaternion.identity, spawnPoints[lane]);
    }

    private GameObject GetRandomZombieType()
    {
        float totalWeight = 0f;
        foreach (float weight in spawnWeights)
        {
            totalWeight += weight;
        }

        float randomWeight = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        //Debug.Log(randomWeight);
        for (int i = 0; i < spawnWeights.Count; i++)
        {
            cumulativeWeight += spawnWeights[i];
            if (randomWeight < cumulativeWeight)
            {
                return zombieTypes[i];
            }
        }

        Debug.Log("default");
        return zombieTypes[0];
    }

    private void AdjustSpawnWeights()
    {
        spawnWeights[0] -= adjustDifficultyBy;
        float remainingAdjust = adjustDifficultyBy;
        for(int i = 1; i < spawnWeights.Count; i++)
        {
            if(i == spawnWeights.Count - 1)
            {
                spawnWeights[i] += remainingAdjust;
                break;
            }
            spawnWeights[i] += remainingAdjust * 0.75f;
            remainingAdjust *= 0.25f;
        }
    }


    private IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(5f);

        zombiesMax++;
        zombiesSpawned++;
        AddSliderValue(wavePercent);

        GameObject FlagZombie = Instantiate(GetRandomZombieType(), spawnPoints[2].position, Quaternion.identity, spawnPoints[2]);

        yield return new WaitForSeconds(2f);

        int zombiesToSpawnPerLane = zombiesPerWave[currentWave] / 5;

        for (int i = 0; i < zombiesToSpawnPerLane; i++)
        {
            foreach (Transform sp in spawnPoints)
            {
                Instantiate(GetRandomZombieType(), sp.position, Quaternion.identity, sp);
                zombiesSpawned++;
            }

            yield return new WaitForSeconds(4f);
        }
        zombiesSpawnedInBetweenWaves = 0;
        zombieCooldownSpawn -= 2.5f;
    }

    void AddSliderValue(float val)
    {
        levelPercent += val;
        levelProgression.value = levelPercent;
    }
}
