using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.Port;

public class Gamemanager : MonoBehaviour
{
    public int sunAmount = 0;
    public static Gamemanager Instance { get; private set; }

    public TextMeshProUGUI sunText;

    public Transform spawningPlants;
    public GameObject[] plantsHUD;
    public GameObject[] plantsToSpawn;
  
    public Transform snapPointsParent; 
    public List<Transform> snapPoints = new List<Transform>();

    private GameObject hoveringPlant;
    private int hoverintPlantID = -1;
    private int hoveringPlantCost;

    public bool isFollowingCursor = false;
 

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
    }
    void SpawnHUDPlants()
    {
        for (int i = 0; i < plantsHUD.Length; i++)
        {
            plantsHUD[i] = Instantiate(plantsHUD[i], spawningPlants.position, Quaternion.identity, spawningPlants);
        }
    }
    public void StartHoveringPlant(int id, int cost)
    {
        if (!isFollowingCursor)
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
