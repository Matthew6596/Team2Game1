using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    public enum CardAbility
    {
        none,kittenExplode,opossumPlayDead
    }

    //Card stats
    public int Energy;
    public int HugPower;
    public float Range;
    public CardAbility Special;
    public bool IsPlayerControlled;

    public void GetHugged(CardScript hugger, bool alwaysLowerEnergy=false)
    {
        //Hugging teammates increases energy, unless alwaysLowerEnergy==true
        if(hugger.IsPlayerControlled==IsPlayerControlled && !alwaysLowerEnergy)
            Energy += hugger.HugPower;
        else
            Energy -= hugger.HugPower;


        if (Energy <= 0) //Card must eep
        {
            Energy = 0;
            CardManager.Discard(gameObject);
        }
    }

    public void Hug(CardScript opponent)
    {
        //Cant hug if too far away
        if (Vector3.Distance(gameObject.transform.position, opponent.transform.position) > Range) return;
        //Cant hug when opossum ability is active
        if (opponent.Special == CardAbility.opossumPlayDead) return;

        //If using the cat explode card
        if (Special == CardAbility.kittenExplode)
        {
            int cnt = CardManager.BoardCards.Count;
            for (int i = 0; i < cnt; i++) //For all cards on the board
            {
                CardScript _card = CardManager.BoardCards[i].GetComponent<CardScript>();
                if (!_card.IsPlayerControlled) //if card is not player controlled (change for balance?)
                {
                    _card.GetHugged(this);
                }
            }
            //Exploded kitten also becomes eepy
            this.GetHugged(this,true);

            //to-do, decrement player turn

            return;
        }

        //Opponent gets hugged
        opponent.GetHugged(this);

        //If hugger is opossum, deactivate play dead ability
        if(Special==CardAbility.opossumPlayDead) Special=CardAbility.none;

        //to-do, decrement player turn
    }
}
