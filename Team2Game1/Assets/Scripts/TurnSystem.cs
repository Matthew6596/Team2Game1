using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    static public int RoundNumber=0;
    static public int TotalPlayerTurns=3;
    static public int PlayerTurnsLeft=3;

    public TMP_Text RoundText;
    public TMP_Text PlayerHpText;
    public TMP_Text BearHpText;
    static TMP_Text roundText;
    static TMP_Text playerText;
    static TMP_Text bearText;

    static public int PlayerEnergy = 100;
    static public int BearEnergy = 100;

    static bool bearTurn = false;
    // Start is called before the first frame update
    void Start()
    {
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
        yield return new WaitForSeconds(0.75f);
        DoBearTurn();
        yield return new WaitForSeconds(0.75f);
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
            //Bear place card on board

        }else if (bearCardsOnBoard < 3 && rand < 50)
        {
            //Bear place card on board
        }
        else
        {
            //Bear attack
        }
    }

    static public void BearGotHugged(int hugPow)
    {
        BearEnergy -= hugPow;

        if (BearEnergy <= 0)
        {
            BearEnergy = 0;
            //Player wins!
        }

        bearText.text = BearEnergy.ToString();
    }
}
