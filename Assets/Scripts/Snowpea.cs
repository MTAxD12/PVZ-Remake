using Unity.VisualScripting;
using UnityEngine;

public class Snowpea : MonoBehaviour
{
    public GameObject bullet;
    [SerializeField] private float rechargeTime = 2f;
    [SerializeField] private float range = 20f;
    private bool isCooldown = false;

    public LayerMask bulletMask;

    private RaycastHit2D currentHit;

    private void Start()
    {
        Debug.Log(transform.position);
    }
    private void Update()
    {
        currentHit = Physics2D.Raycast(transform.position, Vector2.right, range, bulletMask);

        if (currentHit.collider != null && !isCooldown)
        {
            Shoot();
        }
    }

    void Cooldown()
    {
        isCooldown = false;
    }

    void Shoot()
    {
        isCooldown = true;
        Invoke("Cooldown", rechargeTime);

        Vector3 spawnPos = new Vector3(transform.position.x + 0.56f, transform.position.y + 0.113f, transform.position.z);
        GameObject Peabullet = Instantiate(bullet, spawnPos, Quaternion.identity);
    }
}
