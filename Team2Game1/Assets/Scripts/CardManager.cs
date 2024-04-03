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

    public static Vector3 deckPosition,playerHandPos,bearHandPos,cameraPos;

    static Vector3[] boardPositions;
    static int MaxHandCards = 16;
    static Vector3[,] handPositions = new Vector3[MaxHandCards, MaxHandCards]; //16=max number of cards in hand

    static int NumCards=0;
    public static List<GameObject> DeckCards = new();
    public static List<GameObject> PlayerHand = new();
    public static List<GameObject> BearHand = new();
    public static List<GameObject> BoardCards = new();
    public static List<GameObject> DiscardPile = new();

    public static GameObject SelectedCard=null;
    public static bool CardMoving = false;


    public static MonoBehaviour m;
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

    static public void DrawFromDeck(bool bear=false)
    {
        if (DeckCards.Count > 0)
        {
            //Remove card from deck and add to hand
            GameObject c = DeckCards[0];
            DeckCards.Remove(c);
            if (bear)
                BearHand.Add(c);
            else
            {
                PlayerHand.Add(c);
                TurnSystem.PlayerTurnsLeft--;
            }

            SetHandPositions();

        }
    }

    static public void Discard(GameObject card)
    {
        if (PlayerHand.Contains(card)) PlayerHand.Remove(card);
        else if(BearHand.Contains(card)) BearHand.Remove(card);
        else if(BoardCards.Contains(card)) BoardCards.Remove(card);

        DiscardPile.Add(card);
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

    static int shuffleCount = 40; //Number of times that 2 random cards are swapped
    static public void ShuffleCards()
    {
        //Reshuffle discard pile into the deck
        DeckCards.AddRange(DiscardPile);
        DiscardPile.Clear();

        //shuffle
        for (int i = 0; i < shuffleCount; i++)
        {
            //Get 2 random index
            int rand1 = UnityEngine.Random.Range(0, DeckCards.Count - 1);
            int rand2 = UnityEngine.Random.Range(0, DeckCards.Count - 1);

            //Swap the randomly selected cards
            GameObject tmp = DeckCards[rand1];
            DeckCards[rand1] = DeckCards[rand2];
            DeckCards[rand2] = tmp;
        }
    }

    void setHandPositions()
    {
        float cardDistX = 0.8f;
        float randY = 0.05f;
        for(int i=0; i<MaxHandCards; i++)
        {
            for(int j=0; j < MaxHandCards; j++)
            {
                handPositions[i, j] = new Vector3(j * cardDistX, UnityEngine.Random.Range(-randY, randY), 0);
            }
        }
    }
    static public void SetHandPositions()
    {
        //Change card positions
        int nc = PlayerHand.Count - 1;
        for (int i = 0; i < PlayerHand.Count; i++)
        {
            PlayerHand[i].GetComponent<CardScript>().IsPlayerControlled = true;
            MoveCard(PlayerHand[i], handPositions[nc, i] + playerHandPos, cameraPos);
        }
        //Change bear card positions
        nc = BearHand.Count - 1;
        for (int i = 0; i < BearHand.Count; i++)
        {
            BearHand[i].GetComponent<CardScript>().IsPlayerControlled = false;
            MoveCard(BearHand[i], handPositions[nc, i] + bearHandPos, bearHandPos+(Vector3.forward*1000));
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
    static public bool cardBeingExamined = false;
    static public void ExamineCard()
    {
        if (!cardBeingExamined)
        {
            cardBeingExamined = true;
            MoveCard(SelectedCard, cameraPos+Vector3.forward, cameraPos);
        }
        else
        {
            cardBeingExamined = false;
            SetHandPositions();
        }
    }


    static float cardMoveSpd=5f;
    static float distError=0.1f;
    static IEnumerator moveCard(GameObject _card, Vector3 _location,Vector3 lookAt)
    {
        CardMoving = true;
        Transform cardT = _card.transform;
        float dist = Vector3.Distance(cardT.position, _location);
        while (dist > distError) //While card not at desired location
        {
            //Move card towards location
            cardT.position = Vector3.MoveTowards(cardT.position, _location, cardMoveSpd * Time.deltaTime* Vector3.Distance(cardT.position, _location));
            cardT.LookAt(lookAt);
            //Skip to next frame then continue loop
            yield return null;

            dist = Vector3.Distance(cardT.position, _location);
        }
        CardMoving = false;
    }

    static public void MoveCard(GameObject _card, Vector3 _location, Vector3 lookAt)
    {
        m.StartCoroutine(moveCard(_card, _location, lookAt));
    }

    static public int CountPlayerBoardCards(bool bear=false)
    {
        int cnt = 0;
        for(int i=0; i<BoardCards.Count; i++){
            if (BoardCards[i].GetComponent<CardScript>().IsPlayerControlled)
            {
                if(!bear)
                    cnt++;
            }
            else
            {
                if (bear)
                    cnt++;
            }
        }
        return cnt;
    }
    static public List<GameObject> GetPlayerBoardCards(bool bear = false)
    {
        List<GameObject> ret= new();
        for (int i = 0; i < BoardCards.Count; i++)
        {
            if (BoardCards[i].GetComponent<CardScript>().IsPlayerControlled)
            {
                if (!bear)
                    ret.Add(BoardCards[i]);
            }
            else
            {
                if (bear)
                    ret.Add(BoardCards[i]);
            }
        }
        return ret;
    }
}
