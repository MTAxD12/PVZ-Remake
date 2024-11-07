using UnityEngine;

public class SnowBullet : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float speed = 0.5f;
    Zombie attackedZombie;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(speed * Time.fixedDeltaTime, 0, 0);
        if (transform.position.x > 8)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            zombie.TakeDamage(damage);
            zombie.GetSlow(10f);
            Destroy(gameObject);
        }
    }
}
