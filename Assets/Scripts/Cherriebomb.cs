using UnityEngine;

public class Cherriebomb : MonoBehaviour
{
    [SerializeField] float inflateSpeed = 1.0f;
    [SerializeField] float inflateScale = 2.0f;
    [SerializeField] Vector2 explosionSize = new Vector2(5f, 5f);
    [SerializeField] Sprite explosionSprite;
    [SerializeField] LayerMask zombieMask;
    private bool isExploding = false;
    private Vector2 initialSize;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialSize = GetComponent<BoxCollider2D>().size;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<BoxCollider2D>().size = initialSize;
        if(!isExploding)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, transform.localScale * inflateScale, inflateSpeed * Time.deltaTime);
            if (transform.localScale.x >= 1.0f * inflateScale * 0.95f)
            {
                isExploding = true;
                Invoke("Explode", 0.15f);
            }
        }
    }

    private void Explode()
    {
        Vector2 center = transform.position;

        Collider2D[] zombies = Physics2D.OverlapBoxAll(center, explosionSize, 0f, zombieMask);

        foreach(Collider2D zombie in zombies)
        {
            Debug.Log(zombie.transform.name);
            Destroy(zombie.gameObject);
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = explosionSprite;
        transform.localScale = new Vector2(1f, 1f);
        Invoke("DestroyThis", 0.5f);
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
