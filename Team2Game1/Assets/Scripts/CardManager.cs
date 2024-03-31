using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class CardManager : MonoBehaviour
{
    public GameObject[] StartCards;
    public Transform deckTransform,playerHandTransform,bearHandTransform,cameraTransform;

    public Transform[] boardPlacementTransforms;

    static Vector3 deckPosition,playerHandPos,bearHandPos,cameraPos;

    static Vector3[] boardPositions;
    static int MaxHandCards = 16;
    static Vector3[,] handPositions = new Vector3[MaxHandCards, MaxHandCards]; //16=max number of cards in hand

    static int NumCards=0;
    static List<GameObject> DeckCards = new();
    static List<GameObject> PlayerHand = new();
    static List<GameObject> BearHand = new();
    static List<GameObject> BoardCards = new();
    static List<GameObject> DiscardPile = new();


    static MonoBehaviour m = new();
    // Start is called before the first frame update
    void Start()
    {
        m = GameObject.Find("CardManager").GetComponent<MonoBehaviour>();

        deckPosition = deckTransform.position;
        playerHandPos = playerHandTransform.position;
        //bearHandPos = bearHandTransform.position;
        cameraPos = cameraTransform.position;

        NumCards = StartCards.Length;
        setHandPositions();
        createCards();
        ShuffleCards();
        SetDeckPositions();
    }

    static public void DrawFromDeck()
    {
        if (DeckCards.Count > 0)
        {
            //Remove card from deck and add to hand
            GameObject c = DeckCards[0];
            DeckCards.Remove(c);
            PlayerHand.Add(c);
            Debug.Log("Player got card: " + c.name);

            //Change card positions
            int nc = PlayerHand.Count - 1;
            for (int i = 0; i < PlayerHand.Count; i++)
            {
                m.StartCoroutine(MoveCard(PlayerHand[i], handPositions[nc, i] + playerHandPos,cameraPos));
            }
        }
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
        DeckCards.OrderBy(_ => UnityEngine.Random.Range(0, 1000));
    }

    void setHandPositions()
    {
        float cardDistX = 0.4f;
        float randY = 0.05f;
        for(int i=0; i<MaxHandCards; i++)
        {
            for(int j=0; j < MaxHandCards; j++)
            {
                handPositions[i, j] = new Vector3(j * cardDistX, UnityEngine.Random.Range(-randY, randY), 0);
            }
        }
    }
    static public void SetDeckPositions()
    {
        int cardNum = DeckCards.Count - 1;
        for(int i=0; i<DeckCards.Count; i++)
        {
            DeckCards[cardNum].transform.position = Vector3.up*0.02f*i + deckPosition;
            DeckCards[cardNum].transform.rotation = Quaternion.Euler(90, 0, 0);
            cardNum--;
        }
    }

    static float cardMoveSpd=10f;
    static float distError=0.001f;
    static IEnumerator MoveCard(GameObject _card, Vector3 _location,Vector3 lookAt)
    {
        Transform cardT = _card.transform;
        float dist = Vector3.Distance(cardT.position, _location);
        while (dist > distError) //While card not at desired location
        {
            //Move card towards location
            cardT.position = Vector3.MoveTowards(cardT.position, _location, cardMoveSpd * Time.deltaTime);
            cardT.LookAt(lookAt);
            //Skip to next frame then continue loop
            yield return null; 
        }
    }
}
