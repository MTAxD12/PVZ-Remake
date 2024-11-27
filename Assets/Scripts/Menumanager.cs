using UnityEngine;
using UnityEngine.SceneManagement;

public class Menumanager : MonoBehaviour
{
    PlayerData playerData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerData = SaveSystem.LoadPlayerData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartWithPlants()
    {
        playerData.currentLevel = 1;
        SaveSystem.SavePlayerData(playerData);

        SceneManager.LoadScene(1);
    }

    public void RestartWithoutPlants()
    {
        playerData.currentLevel = 1;
        playerData.unlockedCards.Clear();
        playerData.unlockedCards.Add("Peashooter");
        SaveSystem.SavePlayerData(playerData);

        SceneManager.LoadScene(1);
    }
}
