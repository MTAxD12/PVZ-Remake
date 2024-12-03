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

    public void StartGame()
    {
        Debug.Log(playerData.currentLevel);
        if (playerData.currentLevel > 7)
            playerData.currentLevel = 7;
        Debug.Log(playerData.currentLevel);

        SceneManager.LoadScene(playerData.currentLevel);    
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
