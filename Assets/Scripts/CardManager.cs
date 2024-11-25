using UnityEngine;

public class CardManager : MonoBehaviour
{
    public CardCollection cardCollection;

    public GameObject GetCardPrefab(string cardName)
    {
        foreach (CardData card in cardCollection.cards)
        {
            if (card.cardName == cardName)
            {
                return card.cardPrefab;
            }
        }
        Debug.LogWarning($"Card '{cardName}' not found!");
        return null;
    }

    public GameObject GetCardToSpawnPrefab(string cardName)
    {
        foreach (CardData card in cardCollection.cards)
        {
            if (card.cardName == cardName)
            {
                return card.cardToSpawnPrefab;
            }
        }
        Debug.LogWarning($"Card '{cardName}' not found!");
        return null;
    }
}
