using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    static public int RoundNumber=0;
    static public int TotalPlayerTurns=3;
    static public int PlayerTurnsLeft=3;

    public GameObject[] EnemyTiles;
    public GameObject[] PlayerTiles;
    static List<GameObject> enemyTiles=new();
    public static List<GameObject> allTiles=new();

    public TMP_Text RoundText;
    public TMP_Text PlayerHpText;
    public TMP_Text BearHpText;
    static TMP_Text roundText;
    static TMP_Text playerText;
    static TMP_Text bearText;

    static public int PlayerEnergy = 10;
    static public int BearEnergy = 10;

    static bool bearTurn = false;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < EnemyTiles.Length; i++)
        {
            enemyTiles.Add(EnemyTiles[i]);
            allTiles.Add(EnemyTiles[i]);
        }
        for (int i = 0; i < PlayerTiles.Length; i++)
            allTiles.Add(PlayerTiles[i]);
        roundText = RoundText;
        playerText = PlayerHpText;
        bearText = BearHpText;
        BeginRound();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerTurnsLeft<=0 && !bearTurn) CardManager.m.StartCoroutine(DoBearTurns());
    }

    static public void BeginRound()
    {
        RoundNumber++;
        CardManager.ShuffleCards();
        roundText.text = RoundNumber.ToString();
        PlayerTurnsLeft = TotalPlayerTurns;
    }

    static public IEnumerator DoBearTurns()
    {
        bearTurn = true;
        //bear ai
        yield return new WaitForSeconds(0.5f);
        DoBearTurn();
        yield return new WaitForSeconds(1.25f);
        DoBearTurn();
        yield return new WaitForSeconds(1.25f);
        DoBearTurn();
        //
        bearTurn = false;
        playerText.text = PlayerEnergy.ToString();

        //All cards reduce energy by 1 at end of round
        for(int i=0; i<CardManager.BoardCards.Count; i++)
        {
            CardManager.BoardCards[i].GetComponent<CardScript>().GetHugged(1);
        }

        //Begin next round
        BeginRound();
    }

    static void DoBearTurn()
    {
        if (CardManager.BearHand.Count < 2)
        {
            //Bear draw from deck
            CardManager.DrawFromDeck(true);
            return;
        }
        int bearCardsOnBoard = CardManager.CountPlayerBoardCards(true);
        int rand = Random.Range(0, 100);
        if (bearCardsOnBoard == 0)
        {
            bearPlaceCard();

        }else if (bearCardsOnBoard < 5 && rand < 50)
        {
            bearPlaceCard();
        }
        else if (CardManager.CountPlayerBoardCards()>0)
        {
            //Bear attack
            List<GameObject> bearCards = CardManager.GetPlayerBoardCards(true);
            List<GameObject> playerCards = CardManager.GetPlayerBoardCards();
            //No range logic for now
            bearCards[Random.Range(0, bearCards.Count)].GetComponent<CardScript>()
                .Hug(playerCards[Random.Range(0, playerCards.Count)].GetComponent<CardScript>(),true);
        }
        else
        {
            List<GameObject> bearCards = CardManager.GetPlayerBoardCards(true);
            PlayerEnergy -= bearCards[Random.Range(0, bearCards.Count)].GetComponent<CardScript>()
                .HugPower;
            if(PlayerEnergy<=0) MenuScript.LoseGame();
        }
    }
    static void bearPlaceCard()
    {
        //Bear place card on board
            GameObject _card = CardManager.BearHand[Random.Range(0,CardManager.BearHand.Count)];
            bool spaceFound = false;
            while (!spaceFound)
            {
                int ind = Random.Range(0, 5);
                if (!enemyTiles[ind].GetComponent<TileScript>().occupied)
                {
                    CardManager.BearHand.Remove(_card);
                    CardManager.BoardCards.Add(_card);
                    CardManager.MoveCard(_card, enemyTiles[ind].transform.position + Vector3.up * 0.3f + Vector3.back * 0.8f, CardManager.cameraPos);
                    enemyTiles[ind].GetComponent<TileScript>().occupied = true;
                    enemyTiles[ind].GetComponent<TileScript>().card = _card;
                    spaceFound = true;
                    CardManager.SetHandPositions();
                }
            }
    }
    static public void BearGotHugged(int hugPow)
    {
        BearEnergy -= hugPow;

        if (BearEnergy <= 0)
        {
            BearEnergy = 0;
            //Player wins!
            MenuScript.WinGame();
        }

        bearText.text = BearEnergy.ToString();
    }
}
