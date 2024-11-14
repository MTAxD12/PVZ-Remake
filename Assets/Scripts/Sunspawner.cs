using UnityEngine;

public class Sunspawner : MonoBehaviour
{
    public GameObject sunPrefab;
    public float sunSpawnTime = 15f;
    private Gamemanager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManage").GetComponent<Gamemanager>();
        int rng = Random.Range(4, 9);
        sunSpawnTime = Random.Range(6, 13);
        InvokeRepeating("SpawnSun", rng, sunSpawnTime);
    }

    void SpawnSun()
    {
        float randomX = Random.Range(-7.5f, 4f);
        float randomY = Random.Range(-1.5f, 4.3f);


        Vector3 initial = new Vector3(randomX, randomY, -3);
        GameObject sunTemp = Instantiate(sunPrefab, initial, Quaternion.identity);
        sunTemp.GetComponent<Sun>().isNatural = true;
        sunTemp.GetComponent<Sun>().initial = initial;
            
    }
}
