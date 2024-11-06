using System.Collections;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float health = 100f;
    [SerializeField] float speed = 0.05f;
    [SerializeField] float eatingSpeed = 1f;
    [SerializeField] float eatingDamage = 16.6f;

    private Plant eatingPlant;
    private bool isEating;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isEating)
            transform.position -= new Vector3(speed * Time.fixedDeltaTime, 0, 0);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEating)
        {
            eatingPlant = other.GetComponent<Plant>();
            if (eatingPlant != null)
            {
                isEating = true;
                StartCoroutine(EatPlant(eatingPlant));
            }
        }
    }
    private IEnumerator EatPlant(Plant plant)
    {
        while (eatingPlant != null && eatingPlant.health > 0)
        {
            plant.TakeDamage(eatingDamage);
            yield return new WaitForSeconds(eatingSpeed); 
        }

        isEating = false;
        Destroy(eatingPlant);
        eatingPlant = null;
    }
    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Color colorDamaged = gameObject.GetComponent<SpriteRenderer>().color; colorDamaged.g = 0.5f; colorDamaged.b = 0.5f;
        gameObject.GetComponent<SpriteRenderer>().color = colorDamaged;
        Invoke("DamageColor", 0.15f);

        if (health <= 0)
        {
            Die();
        }
    }

    private void DamageColor()
    {
        Color color = gameObject.GetComponent<SpriteRenderer>().color; color.g = 1f; color.b = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = color;
        return;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
