using UnityEngine;
using UnityEngine.UI;
public class Shovel : MonoBehaviour
{
    private Gamemanager gameManager;
    private void Awake()
    {
        gameManager = GameObject.Find("GameManage").GetComponent<Gamemanager>();
    }
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(HoveringShovel);
    }

    void HoveringShovel()
    {
        gameManager.StartHoveringShovel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
