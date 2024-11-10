using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zombiespawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> zombieTypes = new List<GameObject>();
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private Transform spawnPointsParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadSpawnPoints();
        InvokeRepeating("SpawnZombie", 0f, 20f); // 20, 10
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void LoadSpawnPoints()
    {
        foreach (Transform t in spawnPointsParent)
        {
            spawnPoints.Add(t);
        }
    }

    void SpawnZombie()
    {
        int randomLine = Random.Range(0, spawnPoints.Count);
        int randomZombie = Random.Range(0, zombieTypes.Count);
        GameObject Zombie = Instantiate(zombieTypes[randomZombie], spawnPoints[randomLine]);

    }

}
