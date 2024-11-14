using System.Collections;
using UnityEngine;

public class Lawnmower : MonoBehaviour
{
    [SerializeField] private bool isTriggered = false;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float range = 1f;
    [SerializeField] private LayerMask zombieMask;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit2D zombie = Physics2D.Raycast(transform.position, Vector2.right, range, zombieMask);
        if(!isTriggered)
        {
            if (zombie.collider != null)
                isTriggered = true;
        }
        else
        {
            transform.position += new Vector3(speed * Time.fixedDeltaTime, 0, 0);
            transform.Rotate(0, 0, -rotationSpeed * Time.fixedDeltaTime);
            if (zombie.collider != null)
                StartCoroutine(KillZombie(0.15f, zombie.collider.gameObject));

            if (transform.position.x > 10)
                Destroy(gameObject);
        }
    }

    IEnumerator KillZombie(float delay, GameObject zombie)
    {
        yield return new WaitForSeconds(delay);
        if(zombie != null)
            zombie.GetComponent<Zombie>().TakeDamage(1000);
    }
}
