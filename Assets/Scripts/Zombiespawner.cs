using System.Collections.Generic;
using UnityEngine;

public class Zombiespawner : MonoBehaviour
{
    public GameObject[] zombieTypes;
    public Transform[] spawnPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnZombie", 0f, 20f); // 20, 10
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnZombie()
    {
        int randomLine = Random.Range(0, spawnPoints.Length);
        int randomZombie = Random.Range(0, zombieTypes.Length);
        GameObject Zombie = Instantiate(zombieTypes[randomZombie], spawnPoints[randomLine]);

    }

}
