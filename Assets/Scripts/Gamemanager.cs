using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance { get; private set; }

    //camera
    [SerializeField] private GameObject cameraObj;

    //sun
    public int sunAmount = 0;
    [SerializeField] private TextMeshProUGUI sunText;

    //hud plants
    public Transform spawningPlants;
    public List<GameObject> plantsHUD = new List<GameObject>();
    public List<GameObject> plantsToSpawn = new List<GameObject>();

    public GameObject plantToWin;
  
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

    //animations
    public Animator animatorLose;
    public Animator animatorWin;

    //gameStatus
    public bool isWon = false;
    public bool isLost = false;

    //player stats
    private PlayerData playerData;

    //sounds
    [HideInInspector] public AudioSource audioSource;
    public AudioClips clips;
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
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = clips.backgroundMusic;
        audioSource.volume = 0.2f;
        audioSource.Play();
        playerData = SaveSystem.LoadPlayerData();
        Debug.Log($"Nivel curent: {playerData.currentLevel}, Cărți deblocate: {string.Join(", ", playerData.unlockedCards)}");

        SpawnHUDPlants();
        SpawnLawnMowers();
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
                Vector2 mousePos = Camera.main.ScreenToWorldPoint((Vector2)Input.mousePosition);
                hoveringPlant.transform.position = mousePos;

                foreach (Transform tile in snapPoints)
                {
                    tile.GetComponent<SpriteRenderer>().enabled = false;
                }
                if (hit.collider && hit.transform.childCount == 0)
                {
                    hit.collider.GetComponent<SpriteRenderer>().enabled = true;
                    hit.collider.GetComponent<SpriteRenderer>().sprite = plantsToSpawn[hoveringPlantID].GetComponent<SpriteRenderer>().sprite;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if(hit.collider)
                        hit.collider.GetComponent<SpriteRenderer>().enabled = false;

                    StopHoveringPlant();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider && hit.collider.transform.childCount == 0)
                    {
                        hit.collider.GetComponent<SpriteRenderer>().enabled = false;
                        GameObject Plant = Instantiate(plantsToSpawn[hoveringPlantID], hit.collider.transform.position, Quaternion.identity, hit.transform);
                        audioSource.PlayOneShot(clips.plantSFX);

                        MinusSun(hoveringPlantCost);
                        plantsHUD[hoveringPlantID].GetComponent<PlantCard>().StartCooldown();

                        StopHoveringPlant();
                    }
                    else
                    {
                        StopHoveringPlant();
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
                    if (hit.collider && hit.transform.childCount > 0)
                        hit.transform.GetChild(0).GetComponent<SpriteRenderer>().color = normalColor;

                    StopHoveringShovel();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider && hit.transform.childCount > 0)
                    {
                        Destroy(hit.transform.GetChild(0).gameObject);

                        StopHoveringShovel();
                    }
                    else
                    {
                        StopHoveringShovel();
                    }
                }
            }
        }
    }
    private string GetNewCardForLevel(int level)
    {
        // Exemplu de cărți bazate pe nivel
        string[] allCards = { "Peashooter", "Sunflower", "Cherriebomb", "Wallnut", "Potatomine", "Snowpea", "Squash", };
        if (level < allCards.Length)
        {
            return allCards[level];
        }
        else if (level == 7)
        {
            return "OMB";
        }
        else
            return null;
    }
    void SpawnHUDPlants()
    {
        CardManager cardManager = GetComponent<CardManager>();
        for (int i = 0; i < playerData.unlockedCards.Count; i++)
        {
            plantsHUD.Add(Instantiate(cardManager.GetCardPrefab(playerData.unlockedCards[i]), spawningPlants.position, Quaternion.identity, spawningPlants));
            plantsToSpawn.Add(cardManager.GetCardToSpawnPrefab(playerData.unlockedCards[i]));
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
            hoveringPlant = new GameObject("HoveringPlant");
            hoveringPlant.AddComponent<SpriteRenderer>();
            hoveringPlant.GetComponent<SpriteRenderer>().sprite = plantsToSpawn[id].GetComponent<SpriteRenderer>().sprite;
            hoveringPlant.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            isFollowingCursor = true;
        }
    }

    private void StopHoveringPlant()
    {
        UpdateCards();

        //plantsHUD[hoveringPlantID].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
        //plantsHUD[hoveringPlantID].transform.GetChild(1).GetComponent<Image>().color = new Color(1f, 1f, 1f);


        Destroy(hoveringPlant);
        hoveringPlant = null;
        isFollowingCursor = false;
        hoveringPlantID = -1;
        hoveringPlantCost = -1;
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

    private void StopHoveringShovel()
    {
        Destroy(shovel);
        shovel = null;
        isUsingShovel = false;
        
    }


    private void LoadSnapPoints()
    {
        foreach (Transform snapPoint in snapPointsParent)
        {
            snapPoints.Add(snapPoint);
        }
    }

    public void AddSun(int ammount)
    {
        if(ammount == 25)
            audioSource.PlayOneShot(clips.sunClick);
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
        for (int i = 0; i < plantsHUD.Count; i++)
        {
            plantsHUD[i].GetComponent<PlantCard>().UpdateCard();
        }
    }   

    public void WinGame(GameObject lastZombie)
    {
        if (lastZombie == null)
            Debug.Log("e null");
        ZombieSpawner zombieSpawner = GetComponent<ZombieSpawner>();
        if (zombieSpawner.zombiesKilledTotal == zombieSpawner.zombiesMax) // 1 == 1 e ptr teste
        {
            GameObject wonPlant = Instantiate(plantToWin, lastZombie.transform.position, Quaternion.identity, GameObject.Find("CanvasOver").transform);
            Debug.Log("daa");
            if(playerData.currentLevel < 7)
            {
                wonPlant.GetComponent<PlantCard>().enabled = false;
                wonPlant.GetComponent<Button>().onClick.RemoveAllListeners();
                wonPlant.transform.GetChild(1).GetComponent<Slider>().enabled = false;
                wonPlant.transform.GetChild(3).gameObject.SetActive(false);
            }
            wonPlant.AddComponent<WonCard>();
        }
        zombieSpawner.enabled = false;
        GetComponent<Sunspawner>().enabled = false;
        string newCard = GetNewCardForLevel(playerData.currentLevel);
        if (!playerData.unlockedCards.Contains(newCard))
        {
            if(playerData.currentLevel < 7) // 7 == last level
                playerData.unlockedCards.Add(newCard);
            Debug.Log($"Noua carte câștigată: {newCard}");
        }

        // Avansează la nivelul următor
        playerData.currentLevel++;
        SaveSystem.SavePlayerData(playerData);
        isWon = true;
        Debug.Log("U won");
    }
   
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartWithPlants()
    {
        playerData.currentLevel = 1;

        SceneManager.LoadScene(0);
    }

    public void RestartWithoutPlants()
    {
        playerData = null;
        playerData.unlockedCards.Clear();

        SceneManager.LoadScene(0);
    }

    private void RestartScene()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseGamePre()
    {
        StartCoroutine(LoseGame());
    }

    private IEnumerator LoseGame()
    {

        Transform UIParentBehind = GameObject.Find("CanvasBehind").transform; // de continuat poate pui collider in loc sa verifici x
        Transform UIParentOver = GameObject.Find("CanvasOver").transform;
        UIParentBehind.GetChild(0).gameObject.SetActive(false);
        UIParentBehind.GetChild(1).gameObject.SetActive(false);
        UIParentOver.GetChild(0).gameObject.SetActive(false);
        Debug.Log("U lost");


        audioSource.Stop();
        audioSource.PlayOneShot(clips.looseMusic);

        Vector3 desiredPos = new Vector3(cameraObj.transform.position.x - 2.5f, 0f, -10f);
        while (Vector2.Distance(cameraObj.transform.position, desiredPos) > 0.01f)
        {
            cameraObj.transform.position = Vector3.MoveTowards(cameraObj.transform.position, desiredPos, 2f * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        animatorLose.Play("LoseAnimation");

        audioSource.PlayOneShot(clips.scream);

        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

}
