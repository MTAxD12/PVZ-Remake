using System.Collections;
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
    private AudioSource audioSource;
    public AudioClip boomSound;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        initialSize = GetComponent<BoxCollider2D>().size;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<BoxCollider2D>().size = initialSize;
        if(!isExploding)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, transform.localScale * inflateScale, inflateSpeed * Time.deltaTime);
            if (transform.localScale.x >= 1.0f * inflateScale * 0.95f && !isExploding)
            {
                isExploding = true;
                Invoke("Explode", 0.15f);
            }
        }
    }

    private void Explode()
    {
        audioSource.PlayOneShot(boomSound);
        Vector2 center = transform.position;

        Collider2D[] zombies = Physics2D.OverlapBoxAll(center, explosionSize, 0f, zombieMask);

        foreach(Collider2D zombie in zombies)
        {
            //Debug.Log(zombie.transform.name);
            zombie.GetComponent<Zombie>().TakeDamage(1000);
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = explosionSprite;
        transform.localScale = new Vector2(1f, 1f);
        StartCoroutine(DestroyThis());
    }

    private IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
