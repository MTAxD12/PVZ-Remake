using UnityEngine;
using System.Collections;
public class Squash : MonoBehaviour
{
    private bool isInAnimation = false;

    public float range = 1f;
    public float jumpSpeed = 3f;

    public LayerMask zombieMask;

    private RaycastHit2D currentHit;
    private RaycastHit2D closestHit;

    private Vector3 midPos;
    private Vector3 finalPos;

    private AudioSource audioSource;
    public AudioClip[] hmms;
    public AudioClip hit;
    private bool played = false;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        GetComponent<BoxCollider2D>().enabled = false;

    }


    private void FixedUpdate()
    {
        if (!isInAnimation)
        {
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, range, zombieMask);
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, range, zombieMask);


            if (hitRight.collider != null)
            {
                closestHit = hitRight;
            }

            if (hitLeft.collider != null)
            {
                if (closestHit.collider == null || hitLeft.distance < closestHit.distance)
                {
                    closestHit = hitLeft;
                }
            }

            currentHit = closestHit;    

            if (currentHit.collider != null)
            {
                isInAnimation = true;
                StartCoroutine(MoveUpAndDown());
            }
        }
    }

    private IEnumerator MoveUpAndDown()
    {
        audioSource.PlayOneShot(hmms[Random.Range(0, hmms.Length)]);
        yield return new WaitForSeconds(1f);
        midPos = new Vector3(currentHit.collider.transform.position.x - 0.1f, transform.position.y + 1.5f, -1);
        finalPos = new Vector3(currentHit.collider.transform.position.x - 0.1f, transform.position.y, -1);
        while (Vector2.Distance(transform.position, midPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, midPos, jumpSpeed * 1.25f * Time.deltaTime);
            yield return null; 
        }
        audioSource.PlayOneShot(hit);
        GetComponent<BoxCollider2D>().enabled = true;

        while (Vector2.Distance(transform.position, finalPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPos, jumpSpeed * Time.deltaTime);
            yield return null; 
        }

        Invoke("DestroyInstance", 0.5f);
    }

    private void DestroyInstance()
    {
        Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            if(!played)
            {
                //audioSource.PlayOneShot(hit);
                played = true;
            }
            zombie.TakeDamage(zombie.health);
        }
    }
}
