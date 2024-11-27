using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class WonCard : MonoBehaviour
{
    private Gamemanager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManage").GetComponent<Gamemanager>();
        StartCoroutine(MoveDown());
        GetComponent<Button>().onClick.AddListener(TakeWonCard);
    }

    // Update is called once per frame
    void Update()       
    {
        
    }

    private void TakeWonCard()
    {
        StartCoroutine(TakeWonCard2());
    }

    private IEnumerator TakeWonCard2()
    {
        Debug.Log("a intrat");  
        Vector3 finalPos = new Vector3(0, 1.5f, 0);

        while (Vector2.Distance(transform.position, finalPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPos, 2f * Time.deltaTime);
            yield return null;
        }
        gameManager.animatorWin.Play("WinAnimation");
        StartCoroutine(ScaleUp());
    }

    private IEnumerator ScaleUp()
    {
        while(transform.localScale.x < 2f)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, transform.localScale * 2, 0.5f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator MoveDown()
    {
        Vector3 finalPos = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        while (Vector2.Distance(transform.position, finalPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPos, 2f * Time.deltaTime);
            yield return null;
        }
    }
}
