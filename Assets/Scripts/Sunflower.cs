using UnityEditor.Tilemaps;
using UnityEngine;

public class Sunflower : MonoBehaviour
{
    private GameObject sunPrefab;
    public float SunSpawnTime = 10f;
    private void Start()
    {
        sunPrefab = GameObject.Find("GameManage").GetComponent<Sunspawner>().sunPrefab;
        InvokeRepeating("SpawnSun", 3f, SunSpawnTime);
    }

    private void SpawnSun()
    {
        float xPos = transform.position.x + Random.Range(-0.25f, 0.25f);
        bool stanga = false;
        if (xPos - transform.position.x < 0) stanga = true;

        GameObject sunTemp = Instantiate(sunPrefab, new Vector3(xPos, transform.position.y + 0.5f, -2), Quaternion.identity);

        sunTemp.GetComponent<Sun>().isNatural = false;
        sunTemp.GetComponent<Sun>().initial = new Vector2(xPos, transform.position.y + 0.5f);
        sunTemp.GetComponent<Sun>().stanga = stanga;
    }
}
