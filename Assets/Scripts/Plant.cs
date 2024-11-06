using UnityEngine;

public class Plant : MonoBehaviour
{
    public float health = 100f;
    private GameObject draggingCopy;

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.GetComponentInParent<SnapPoint>().isOccupied = false;
        Destroy(gameObject);
    }
}
