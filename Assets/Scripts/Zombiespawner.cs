using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public GameObject waveText;

    public int[] zombiesInBetweenWaves;
    public int zombiesSpawnedInBetweenWaves;

    public int zombiesSpawned = 0;
    public int zombiesKilledCurrent = 0;
    public int zombiesKilledTotal = 0;
    public int zombiesMax = 30;
    private int spawn2 = 0;

    public float zombieCooldownSpawn = 20f;
    public float zombieRemainingCooldown = 0f;
    public bool isCooldown = true;

    public bool isWon = false;

    private Gamemanager gameManager;

    private Animator waveAnim;

    private void Start()
    {
        waveText = Instantiate(waveText, new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("CanvasOver").transform);
        waveText.name = "WaveText";
        waveAnim = GameObject.Find("WaveText").GetComponent<Animator>();    
     
        //progress bar
        levelProgression.maxValue = 100f;
        for (int i = 0; i < wavesCount; i++)
            Instantiate(flagPrefab, flagParent.position, Quaternion.identity, flagParent);

        wavePercent = 13f;
        betweenWavePerc = (100 - (wavesCount * wavePercent))/wavesCount;
      
        for (int i = 0; i < wavesCount; i++)
        {
            zombiePercentPerBW.Add(betweenWavePerc / zombiesInBetweenWaves[i]);
            //Debug.Log(zombiePercentPerBW[i]);
        }

        //other
        gameManager = gameObject.GetComponent<Gamemanager>();
        zombieCooldownSpawn = 20f;
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
            if (zombiesKilledCurrent >= zombiesInBetweenWaves[currentWave] && !activeWave)
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
        }
    }

    private void SpawnZombie()
    {
        for(int i = 0; i <= spawn2; i++)
        {
            zombiesSpawned++;
            zombiesSpawnedInBetweenWaves++;
            AddSliderValue(zombiePercentPerBW[currentWave]);
        }
        isCooldown = true;

        if (zombiesSpawned == 2)
            zombieCooldownSpawn = 12.5f;
        if (zombiesSpawned == 6)
            zombieCooldownSpawn = 10f;
        zombieRemainingCooldown = zombieCooldownSpawn;

        for (int i = 0; i <= spawn2; i++)
        {
            int lane = Random.Range(0, spawnPoints.Count);
            GameObject zombieToSpawn = GetRandomZombieType();
            string zombieName = zombieToSpawn.name;
            zombieToSpawn = Instantiate(zombieToSpawn, spawnPoints[lane].position, Quaternion.identity, spawnPoints[lane]);
            zombieToSpawn.name = zombieName;
        }
        if (zombiesSpawned == 4)
            spawn2 = 1;
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
        yield return new WaitForSeconds(1f);
        waveAnim.SetBool("PlayAnim", true);
        waveText.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(5f);

        waveAnim.SetBool("PlayAnim", false);

        zombiesSpawnedInBetweenWaves = 0;
        zombieCooldownSpawn -= 2.5f;
        zombieRemainingCooldown = zombieCooldownSpawn;
        zombiesMax++;
        zombiesSpawned++;

        AddSliderValue(wavePercent);
 
        GameObject FlagZombie = GetRandomZombieType();
        string zombieName = FlagZombie.name;
        FlagZombie = Instantiate(GetRandomZombieType(), spawnPoints[spawnPoints.Count / 2].position, Quaternion.identity, spawnPoints[spawnPoints.Count/2]);
        FlagZombie.name = zombieName;
        FlagZombie.GetComponent<Zombie>().isWaveZombie = true;
        yield return new WaitForSeconds(2f);

        int zombiesToSpawnPerLane = zombiesPerWave[currentWave] / spawnPoints.Count;

        for (int i = 0; i < zombiesToSpawnPerLane; i++)
        {
            foreach (Transform sp in spawnPoints)
            {
                FlagZombie = GetRandomZombieType();
                zombieName = FlagZombie.name;
                FlagZombie = Instantiate(FlagZombie, sp.position, Quaternion.identity, sp);
                FlagZombie.name = zombieName;
                FlagZombie.GetComponent<Zombie>().isWaveZombie = true;
                zombiesSpawned++;
            }

            yield return new WaitForSeconds(4f);
        }
    }

    void AddSliderValue(float val)
    {
        levelPercent += val;
        levelProgression.value = levelPercent;
    }
}
