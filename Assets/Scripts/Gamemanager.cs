using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance { get; private set; }

    //sun
    public int sunAmount = 0;
    [SerializeField] private TextMeshProUGUI sunText;

    //hud plants
    [SerializeField] private Transform spawningPlants;
    [SerializeField] private GameObject[] plantsHUD;
    [SerializeField] private GameObject[] plantsToSpawn;
  
    //snap points
    [SerializeField] private Transform snapPointsParent; 
    [SerializeField] private List<Transform> snapPoints = new List<Transform>();
    [SerializeField] private LayerMask tileMask;

    //lawn mowers
    [SerializeField] private GameObject lawnMowerPrefab;
    [SerializeField] private Transform lawnMowersParent;
    [SerializeField] private List<Transform> lawnMowerSpawnPoints = new List<Transform>();

    //shovel
    [SerializeField] private Sprite shovelSprite;
    private GameObject shovel;
    private Color shovelColor = new Color(1f, 0.5f, 0.5f);
    private Color normalColor = new Color(1f, 1f, 1f);

    //hovering plant
    private GameObject hoveringPlant;
    private int hoveringPlantID = -1;
    private int hoveringPlantCost;

    //bools for update
    public bool isFollowingCursor = false;
    public bool isUsingShovel = false;

    // animations
    public Animator animator;

    //gameStatus
    public bool isWon = false;
    public bool isLost = false;


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
        //SpawnLawnMowers();
        LoadSnapPoints();
        AddSun(50);
    }
    private void Update()
    {
        if (isFollowingCursor || isUsingShovel)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, tileMask);

            if (isFollowingCursor)
            {
                foreach (Transform tile in snapPoints)
                {
                    tile.GetComponent<SpriteRenderer>().enabled = false;
                }
                if (hit.collider)
                {
                    hit.collider.GetComponent<SpriteRenderer>().enabled = true;
                    hit.collider.GetComponent<SpriteRenderer>().sprite = plantsToSpawn[hoveringPlantID].GetComponent<SpriteRenderer>().sprite;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    isFollowingCursor = false;
                    if(hit.collider)
                        hit.collider.GetComponent<SpriteRenderer>().enabled = false;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider && hit.collider.transform.childCount == 0)
                    {
                        hit.collider.GetComponent<SpriteRenderer>().enabled = false;
                        GameObject Plant = Instantiate(plantsToSpawn[hoveringPlantID], hit.collider.transform.position, Quaternion.identity, hit.transform);
                        isFollowingCursor = false;

                        MinusSun(hoveringPlantCost);
                        plantsHUD[hoveringPlantID].GetComponent<PlantCard>().StartCooldown();
                        hoveringPlantID = -1;
                        hoveringPlantCost = -1;
                    }
                    else
                    {
                        Debug.Log("e ocupat nene");
                    }
                }
            }
            if (isUsingShovel)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
                shovel.transform.position = mousePos;

                foreach (Transform tile in snapPoints)
                {
                    if (tile.transform.childCount > 0)
                    {
                        tile.GetChild(0).GetComponent<SpriteRenderer>().color = normalColor;
                    }
                }

                if (hit.collider && hit.collider.transform.childCount > 0)
                {
                    hit.collider.transform.GetChild(0).GetComponent<SpriteRenderer>().color = shovelColor;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Destroy(shovel);
                    shovel = null;
                    isUsingShovel = false;
                    if (hit.collider && hit.transform.childCount > 0)
                        hit.transform.GetChild(0).GetComponent<SpriteRenderer>().color = normalColor;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider && hit.transform.childCount > 0)
                    {
                        Destroy(hit.transform.GetChild(0).gameObject);
                        isUsingShovel = false;
                        Destroy(shovel);
                        shovel = null;
                    }
                    else
                    {
                        Debug.Log("nu e nici o planta aici");
                    }
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
            hoveringPlantID = id;
            isFollowingCursor = true;
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

    public void WinGame()
    {
        if(!isWon)
        {
            isWon = true;

            Debug.Log("U won");
        }
    }

    public void LoseGame()
    {
        if(!isLost)
        {
            isLost = true;

            Transform UIParent = GameObject.Find("Canvas").transform; // de continuat poate pui collider in loc sa verifici x
            UIParent.GetChild(0).gameObject.SetActive(false);
            UIParent.GetChild(1).gameObject.SetActive(false);
            UIParent.GetChild(2).gameObject.SetActive(false);


            animator.Play("LoseAnimation");
            Invoke("RestartScene", 5f);
            Debug.Log("U lost");
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
