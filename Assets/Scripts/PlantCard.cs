using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantCard : MonoBehaviour
{
    public int id;
    public int cost;

    private Slider sliderCooldown;
    public float remainingCooldownTime;
    public float cooldownDuration = 10f;
    private bool isCooldown = false;

    private Gamemanager gameManager;
    private Image imagineCard;
    private Image backgroundImagine;

    private Color noSun = new Color(0.5f, 0.5f, 0.5f);
    private Color hasSun = new Color(1f, 1f, 1f);
    private Color cooldown = new Color(0.3f, 0.3f, 0.3f);

    private void Awake()
    {
        gameManager = GameObject.Find("GameManage").GetComponent<Gamemanager>();
        backgroundImagine = transform.GetChild(0).gameObject.GetComponent<Image>();
        imagineCard = transform.GetChild(1).gameObject.GetComponent<Image>();
        sliderCooldown = transform.GetChild(1).gameObject.GetComponent<Slider>();
        transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = cost.ToString();
        if (sliderCooldown == null)
            Debug.Log("null" + gameObject.name);
    }
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(BuyPlant);
        sliderCooldown.value = 0;
        sliderCooldown.maxValue = cooldownDuration;
        if (cooldownDuration > 30) sliderCooldown.wholeNumbers = true;
        else sliderCooldown.wholeNumbers = false;
    }

    private void BuyPlant()
    {
        if (!isCooldown)
        {
            if (gameManager.sunAmount >= cost)
            {
                gameManager.StartHoveringPlant(id, cost);
                backgroundImagine.color = cooldown;
                imagineCard.color = cooldown;
            }
            else
            Debug.Log("Not enough sun");
        }
        else
            Debug.Log("The plant is on cooldown");  
    }

    public void StartCooldown()
    {
        isCooldown = true;
        remainingCooldownTime = cooldownDuration;
        Invoke("Cooldown", cooldownDuration);
        UpdateCard();
    }

    private void Cooldown()
    {
        isCooldown = false;
        remainingCooldownTime = 0;
        UpdateCard();
    }

    public void UpdateCard()
    {
        if (gameManager.sunAmount < cost || isCooldown)
        {
            imagineCard.color = noSun;
            backgroundImagine.color = noSun;
        }
        else if (gameManager.sunAmount >= cost && !isCooldown)
        {
            imagineCard.color = hasSun;
            backgroundImagine.color = hasSun;
        }
    }

    private void Update()
    {
        if (isCooldown)
        {
            remainingCooldownTime -= Time.deltaTime;
            sliderCooldown.value = remainingCooldownTime;

            if (remainingCooldownTime <= 0)
            {
                Cooldown();
            }
        }
    }




}
