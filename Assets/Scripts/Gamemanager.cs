using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class Gamemanager : MonoBehaviour
{
    public int sunAmount = 0;
    public static Gamemanager Instance { get; private set; }

    //sun
    [SerializeField] private TextMeshProUGUI sunText;

    //hud plants
    [SerializeField] private Transform spawningPlants;
    [SerializeField] private GameObject[] plantsHUD;
    [SerializeField] private GameObject[] plantsToSpawn;
  
    //snap points
    [SerializeField] private Transform snapPointsParent; 
    [SerializeField] private List<Transform> snapPoints = new List<Transform>();

    //lawn mowers
    [SerializeField] private GameObject lawnMowerPrefab;
    [SerializeField] private Transform lawnMowersParent;
    [SerializeField] private List<Transform> lawnMowerSpawnPoints = new List<Transform>();

    //shovel
    [SerializeField] private Sprite shovelSprite;
    private GameObject shovel;
    private GameObject hoveringDestroyingPlant;
    private Color shovelColor = new Color(1f, 0.5f, 0.5f);
    private Color normalColor = new Color(1f, 1f, 1f);
    private Transform currentSnapPoint;
    private Transform prevSnapPoint;

    //hovering plant
    private GameObject hoveringPlant;
    private int hoverintPlantID = -1;
    private int hoveringPlantCost;

    //bools for update
    public bool isFollowingCursor = false;
    public bool isUsingShovel = false;
 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SpawnHUDPlants();
        SpawnLawnMowers();
        LoadSnapPoints();
        AddSun(50);
    }
    private void Update()
    {
        if (isFollowingCursor)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
            hoveringPlant.transform.position = FindNearestSnapPoint(mousePos).transform.position;
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(hoveringPlant);
                hoveringPlant = null;
                isFollowingCursor = false;
            }
            if(Input.GetMouseButtonDown(0))
            {
                Transform snapPoint = FindNearestSnapPoint(hoveringPlant.transform.position);
                if (snapPoint.childCount == 0)
                {
                    GameObject Plant = Instantiate(plantsToSpawn[hoverintPlantID], hoveringPlant.transform.position,
                    Quaternion.identity, snapPoint);
                    isFollowingCursor = false;
                    Destroy(hoveringPlant);
                    hoveringPlant = null;

                    MinusSun(hoveringPlantCost);
                    plantsHUD[hoverintPlantID].GetComponent<PlantCard>().StartCooldown();
                    hoverintPlantID = -1;
                    hoveringPlantCost = -1;
                }
                else
                {
                    Debug.Log("e ocupat nene");
                }
            }
        }
        if(isUsingShovel)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
            shovel.transform.position = mousePos;

            currentSnapPoint = FindNearestSnapPoint(mousePos);
            if (currentSnapPoint.childCount > 0)
                currentSnapPoint.GetChild(0).GetComponent<SpriteRenderer>().color = shovelColor;

            if (currentSnapPoint != prevSnapPoint)
            {
                if (prevSnapPoint != null && prevSnapPoint.childCount > 0)
                {
                    prevSnapPoint.GetChild(0).GetComponent<SpriteRenderer>().color = normalColor;
                }
                prevSnapPoint = currentSnapPoint;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(shovel);
                shovel = null;
                isUsingShovel = false;
                if (currentSnapPoint.childCount > 0)
                    currentSnapPoint.GetChild(0).GetComponent<SpriteRenderer>().color = normalColor;

                if (prevSnapPoint.childCount > 0)
                    prevSnapPoint.GetChild(0).GetComponent<SpriteRenderer>().color = normalColor;


            }
            if (Input.GetMouseButtonDown(0))
            {
                Transform snapPoint = FindNearestSnapPoint(mousePos);
                if (snapPoint.childCount > 0)
                {
                    Destroy(snapPoint.GetChild(0).gameObject);
                    isUsingShovel = false;
                    Destroy(shovel);
                    Destroy(hoveringPlant);
                    shovel = null;
                    hoveringPlant = null;
                }
                else
                {
                    Debug.Log("nu e nici o planta aici");
                }
            }
        }
    }
    void SpawnHUDPlants()
    {
        for (int i = 0; i < plantsHUD.Length; i++)
        {
            plantsHUD[i] = Instantiate(plantsHUD[i], spawningPlants.position, Quaternion.identity, spawningPlants);
        }
    }

    void SpawnLawnMowers()
    {
        foreach(Transform t in lawnMowersParent.transform)
        {
            GameObject lm = Instantiate(lawnMowerPrefab, t.position, Quaternion.identity, t);

        }
    }

    public void StartHoveringPlant(int id, int cost)
    {
        if (!isFollowingCursor && !isUsingShovel)
        {
            hoveringPlantCost = cost;
            isFollowingCursor = true;
            hoveringPlant = new GameObject("SpawningACard");

            hoveringPlant.AddComponent<SpriteRenderer>();
            hoveringPlant.GetComponent<SpriteRenderer>().sprite = plantsToSpawn[id].GetComponent<SpriteRenderer>().sprite;

            Color color = hoveringPlant.GetComponent<SpriteRenderer>().color; color.a = 0.5f;
            hoveringPlant.GetComponent<SpriteRenderer>().color = color;

            hoveringPlant.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

            hoverintPlantID = id;

        }
    }

    public void StartHoveringShovel()
    {
        if (!isFollowingCursor && !isUsingShovel)
        {
            isUsingShovel = true;
            shovel = new GameObject("Shovel");
            shovel.AddComponent<SpriteRenderer>();
            shovel.GetComponent<SpriteRenderer>().sprite = shovelSprite;
            currentSnapPoint = null;
            prevSnapPoint = null;
        }
    }
    public void PlaceOnSnapPoint(Transform snapPoint)
    {
        if (isFollowingCursor && hoveringPlant != null)
        {
            Destroy(hoveringPlant);
            hoveringPlant = null;
            isFollowingCursor = false;

            GameObject Plant = Instantiate(plantsToSpawn[hoverintPlantID], snapPoint.position, Quaternion.identity, snapPoint);
            //Plant.transform.position = snapPoint.position;
            //Plant.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

            MinusSun(hoveringPlantCost);
            plantsHUD[hoverintPlantID].GetComponent<PlantCard>().StartCooldown();
            hoverintPlantID = -1;
            hoveringPlantCost = -1;
        }
    }
    private void LoadSnapPoints()
    {
        foreach (Transform snapPoint in snapPointsParent)
        {
            snapPoints.Add(snapPoint);
        }
    }

    public Transform FindNearestSnapPoint(Vector2 position)
    {
        Transform nearestPoint = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform snapPoint in snapPoints)
        {
            float distance = Vector2.Distance(new Vector2(position.x, position.y), new Vector2(snapPoint.position.x, snapPoint.position.y));
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPoint = snapPoint;
            }
        }

        return nearestPoint;
    }

    public void AddSun(int ammount)
    {
        sunAmount += ammount;
        sunText.text = sunAmount.ToString();
        UpdateCards();
    }
    public void MinusSun(int ammount)
    {
        sunAmount -= ammount;
        sunText.text = sunAmount.ToString();
        UpdateCards();
    }

    private void UpdateCards()
    {
        for (int i = 0; i < plantsHUD.Length; i++)
        {
            plantsHUD[i].GetComponent<PlantCard>().UpdateCard();
        }
    }


}
