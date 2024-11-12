using UnityEngine;

public class Zombiecone : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    bool isLow = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLow)
        {
            if (gameObject.GetComponent<Zombie>().health <= 100)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = normalSprite;
                isLow = true;
            }
        }
        
    }
}
