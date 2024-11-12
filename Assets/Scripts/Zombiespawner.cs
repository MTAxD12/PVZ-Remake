using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> zombieTypes = new List<GameObject>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    public int totalWaves = 5;
    public float timeBetweenWaves = 10f;
    public float spawnInterval = 1.5f;

    private int currentWave = 1;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        if (currentWave == 1)
        {
            yield return new WaitForSeconds(20f);
        }

            while (currentWave <= totalWaves)
        {
            int zombiesInWave = currentWave * 3 + Random.Range(0, 3); // Crește numărul de zombii la fiecare val
            float waveDuration = 0;

            while (waveDuration < timeBetweenWaves && zombiesInWave > 0)
            {
                yield return StartCoroutine(SpawnZombieInLane());
                waveDuration += spawnInterval;
                zombiesInWave--;
            }

            yield return new WaitForSeconds(timeBetweenWaves); // Pauză între valuri
            currentWave++;
        }
    }

    IEnumerator SpawnZombieInLane()
    {
        int lane = Random.Range(0, spawnPoints.Count); // Alege un lane aleatoriu
        GameObject zombie = ChooseZombie();

        Instantiate(zombie, spawnPoints[lane].position, Quaternion.identity, spawnPoints[lane]);
        yield return new WaitForSeconds(spawnInterval);
    }

    GameObject ChooseZombie()
    {
        int waveThreshold = Mathf.Min(currentWave, zombieTypes.Count);  // Ajustează tipurile de zombii în funcție de val
        int zombieIndex = Random.Range(0, waveThreshold); // Selectează un tip de zombie din listă, în funcție de valul curent

        return zombieTypes[zombieIndex];
    }
}
