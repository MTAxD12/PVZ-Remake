using UnityEngine;

public class SnowBullet : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float speed = 0.5f;
    private Zombie attackedZombie = null;

    void FixedUpdate()
    {
        transform.position += new Vector3(speed * Time.fixedDeltaTime, 0, 0);
        if (transform.position.x > 8)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackedZombie == null && other.TryGetComponent<Zombie>(out Zombie zombie))
        {
            attackedZombie = zombie;
            zombie.TakeDamage(damage);
            zombie.GetSlow(10f);
            Destroy(gameObject);
        }
    }
}
