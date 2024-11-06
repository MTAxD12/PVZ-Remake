using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    [SerializeField]
    private Gamemanager gameManager;
    public bool isOccupied = false;

    private void Awake()
    {
        Gamemanager gameManager = GameObject.Find("GameManage").GetComponent<Gamemanager>();
    }

    /*private void OnMouseDown()
    {
        GameObject activePlantCard = GameObject.Find("SpawningACard");

        if (activePlantCard != null && gameManager.isFollowingCursor)
        {
            if (transform.childCount > 0)
            {
                Debug.Log("ma nebunule e ocupat");
            }
            else
            {
                gameManager.PlaceOnSnapPoint(transform);
                isOccupied = true;
                gameManager.isFollowingCursor = false;

            }
        }
    }*/
}
