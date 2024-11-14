using JetBrains.Annotations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering;

public class Sun : MonoBehaviour
{
    Gamemanager gameManager;
    public bool isNatural;
    public bool stanga;
    public Vector3 initial;
    public Vector3 desired1;
    public Vector3 desired2;
    private bool movingUp = true; //daca nu este natural


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManage").GetComponent<Gamemanager>();
        Invoke("Despawn", 8f);

        if(!isNatural)
        {
            desired1 = new Vector3(initial.x, initial.y + 0.5f, -3);
            float ds2off = Random.Range(-0.25f, 0f);

            if (stanga)
                ds2off = Random.Range(-0.25f, 0f);
            else
                ds2off = Random.Range(0f, 0.25f);

            desired2 = new Vector3(desired1.x + ds2off, desired1.y - 1f, -3);

        }
        else
        {   
            desired1 = new Vector3(initial.x, initial.y - 3, -3);
        }
    }

    private void OnMouseDown()
    {
        gameManager.AddSun(25);
        Destroy(gameObject);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
    void FixedUpdate()
    {
        if(isNatural)
        {
            if (Vector2.Distance(transform.position, desired1) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, desired1, 0.5f*Time.fixedDeltaTime);
            }

        }
        else
        {
            if(movingUp)
            {
                if (Vector2.Distance(transform.position, desired1) > 0.01)
                {
                    transform.position = Vector3.MoveTowards(transform.position, desired1, 1.5f * Time.fixedDeltaTime);
                }
                else
                {
                    movingUp = false;
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, desired2) > 0.01)
                {
                    transform.position = Vector3.MoveTowards(transform.position, desired2, 1.5f * Time.fixedDeltaTime);
                }
            }
        }
    }
}
