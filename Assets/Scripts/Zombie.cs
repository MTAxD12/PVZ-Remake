using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float health = 100f;
    public float slowTime = 0f;
    private bool isSlowed = false;

    private float currentSpeed;
    [SerializeField] float speed = 0.05f;
    private float slowedSpeed;

    private float currentEatingSpeed;
    [SerializeField] float eatingSpeed = 1f;
    private float slowedEatingSpeed;

    [SerializeField] float eatingDamage = 16.6f;

    private Plant eatingPlant;
    private bool isEating;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSpeed = speed;
        currentEatingSpeed = eatingSpeed;
        slowedSpeed = 0.5f * speed;
        slowedEatingSpeed = 2 * eatingSpeed;
    }

    private void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSlowed)
        {
            slowTime -= Time.deltaTime;

            if (slowTime <= 0)
            {
                slowTime = 0;
                SlowCooldown();
            }
        }

        if (!isEating)
            transform.position -= new Vector3(currentSpeed * Time.fixedDeltaTime, 0, 0);

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
            yield return new WaitForSeconds(currentEatingSpeed); 
        }

        isEating = false;
        Destroy(eatingPlant);
        eatingPlant = null;
    }

    public void GetSlow(float timeLeft)
    {
        isSlowed = true;
        currentSpeed = slowedSpeed;
        currentEatingSpeed = slowedEatingSpeed;
        Color slowedColor = gameObject.GetComponent<SpriteRenderer>().color; slowedColor.r = 0.35f; slowedColor.b = 0.8f;
        gameObject.GetComponent<SpriteRenderer>().color = slowedColor;
        slowTime = 10f;
    }

    private void SlowCooldown()
    {
        isSlowed = false;
        currentSpeed = speed;
        currentEatingSpeed = eatingSpeed;
        Color color = gameObject.GetComponent<SpriteRenderer>().color; color.r = 1f; color.b = 1f;
        gameObject.GetComponent<SpriteRenderer>().color = color;
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
