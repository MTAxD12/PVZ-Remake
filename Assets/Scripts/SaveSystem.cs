using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    private static string savePath = Path.Combine(Application.dataPath, "playerData.json");

    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true); // Serializare în JSON
        File.WriteAllText(savePath, json);           // Scrierea în fișier
        Debug.Log($"Datele jucătorului au fost salvate: {savePath}");
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath); // Citirea din fișier
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Datele jucătorului au fost încărcate!");
            return data;
        }
        else
        {
            Debug.Log("Nu există date salvate. Creare de date noi.");
            return new PlayerData { currentLevel = 1, unlockedCards = new List<string> { "Peashooter" } }; // Date implicite
        }
    }
}
