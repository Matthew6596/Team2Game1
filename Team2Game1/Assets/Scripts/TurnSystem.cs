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
        if(PlayerTurnsLeft<=0 && !bearTurn) DoBearTurns();
    }

    static public void BeginRound()
    {
        RoundNumber++;
        roundText.text = RoundNumber.ToString();
        PlayerTurnsLeft = TotalPlayerTurns;
    }

    static public void DoBearTurns()
    {
        bearTurn = true;
        //bear ai

        //
        bearTurn = false;
        playerText.text = PlayerEnergy.ToString();
        BeginRound();
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
