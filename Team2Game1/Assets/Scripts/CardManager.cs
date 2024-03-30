using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject[] StartCards;
    static int NumCards=0;
    static List<GameObject> DeckCards = new();
    static List<GameObject> PlayerHand = new();
    static List<GameObject> BearHand = new();
    static List<GameObject> BoardCards = new();
    static List<GameObject> DiscardPile = new();

    // Start is called before the first frame update
    void Start()
    {
        NumCards = StartCards.Length;
        createCards();
        ShuffleCards();
    }

    static public void DrawFromDeck()
    {
        GameObject c = DeckCards[0];
        DeckCards.Remove(c);
        PlayerHand.Add(c);
        Debug.Log("Player got card: " + c.name);
    }

    void createCards()
    {
        for(int i=0; i<NumCards; i++)
        {
            createCard(StartCards[i]);
        }
    }
    void createCard(GameObject cardPrefab)
    {
        DeckCards.Add(Instantiate(cardPrefab));
    }

    static public void ShuffleCards()
    {
        DeckCards = DeckCards.OrderBy(_ => Random.Range(0, 1000)).ToList<GameObject>();
    }
}
