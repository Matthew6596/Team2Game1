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
    static TMP_Text roundText;

    static bool bearTurn = false;
    // Start is called before the first frame update
    void Start()
    {
        roundText = RoundText;
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
        BeginRound();
    }
}
