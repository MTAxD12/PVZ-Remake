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
        midPos = new Vector3(currentHit.collider.transform.position.x - 0.2f, transform.position.y + 1.5f, -1);
        finalPos = new Vector3(currentHit.collider.transform.position.x - 0.2f, transform.position.y, -1);
        while (Vector2.Distance(transform.position, midPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, midPos, jumpSpeed * 1.25f * Time.deltaTime);
            yield return null; 
        }

        while (Vector2.Distance(transform.position, finalPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPos, jumpSpeed * Time.deltaTime);
            yield return null; 
        }

        Invoke("DestroyInstance", 0.2f);
    }

    private void DestroyInstance()
    {
        Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            zombie.TakeDamage(zombie.health);
        }
    }
}
