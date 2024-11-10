using System.Collections.Generic;
using UnityEngine;

public class Potatomine : MonoBehaviour
{
    [SerializeField] private Sprite initialSprite;
    [SerializeField] private Sprite grownSprite;
    [SerializeField] private Sprite boomSprite;
    [SerializeField] private float growTime = 14f;
    [SerializeField] private Vector2 explosionSize = new Vector2(1f, 0.5f);
    [SerializeField] private LayerMask zombieMask;
    private Collider2D[] zombies;
    private Vector2 center;
    bool isGrown = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = initialSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGrown)
        {
            growTime -= Time.deltaTime;

            if(growTime < 0)
            {
                growTime = 0;
                Grow();
            }
                
        }
        else
        {
            center = transform.position;

            zombies = Physics2D.OverlapBoxAll(center, explosionSize, 0f, zombieMask);

            if (zombies.Length > 0)
                Boom();
        }
    }

    void Boom()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = boomSprite;
        transform.localScale = new Vector2(0.5f, 0.5f);

        foreach (Collider2D zombie in zombies)
        {
            Debug.Log(zombie.transform.name);
            Destroy(zombie.gameObject);
        }
        Invoke("DestroyThis", 0.5f);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }

    void Grow()
    {
        isGrown = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = grownSprite;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
    }


}
