using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    public enum CardAbility
    {
        none,kittenExplode,opossumPlayDead,caterpillarEepTouch
    }

    //Card stats
    public int Energy;
    public int HugPower;
    public float Range;
    public CardAbility Special;
    public bool IsPlayerControlled=true;

    public void GetHugged(CardScript hugger, bool alwaysLowerEnergy=false)
    {
        //Hugging teammates increases energy, unless alwaysLowerEnergy==true
        if(hugger.IsPlayerControlled==IsPlayerControlled && !alwaysLowerEnergy)
            Energy += hugger.HugPower;
        else
            Energy -= hugger.HugPower;

        //If caterpillar, hugger also gets eepy by 1
        if (Special == CardAbility.caterpillarEepTouch)
            hugger.GetHugged(1);

        if (Energy <= 0) //Card must eep
        {
            Energy = 0;
            CardManager.Discard(gameObject);
            gameObject.transform.position = Vector3.down*100;
        }
    }

    public void GetHugged(int hugPow)
    {
        Energy -= hugPow;

        if (Energy <= 0) //Card must eep
        {
            Energy = 0;
            CardManager.Discard(gameObject);
            gameObject.transform.position = Vector3.down * 100;
        }
    }

    public void Hug(CardScript opponent,bool bear=false)
    {
        //Cant hug if too far away
        //if (Vector3.Distance(gameObject.transform.position, opponent.transform.position) > Range) return;
        //Cant hug when opossum ability is active
        if (opponent.Special == CardAbility.opossumPlayDead) return;

        //If using the cat explode card
        if (Special == CardAbility.kittenExplode)
        {
            int cnt = CardManager.BoardCards.Count;
            CardScript[] cards = new CardScript[cnt];
            for(int i=0; i<cnt; i++)
                cards[i] = CardManager.BoardCards[i].GetComponent<CardScript>();
            for (int i = 0; i < cnt; i++) //For all cards on the board
            {
                //CardScript _card = CardManager.BoardCards[i].GetComponent<CardScript>();
                CardScript _card = cards[i];
                if (!_card.IsPlayerControlled) //if card is not player controlled (change for balance?)
                {
                    _card.GetHugged(this);
                }
            }
            //Exploded kitten also becomes eepy
            this.GetHugged(this,true);

            //to-do, decrement player turn
            if(!bear)
                TurnSystem.PlayerTurnsLeft--;

            return;
        }

        //Opponent gets hugged
        opponent.GetHugged(this);

        //If hugger is opossum, deactivate play dead ability
        if(Special==CardAbility.opossumPlayDead) Special=CardAbility.none;

        //to-do, decrement player turn
        if (!bear)
            TurnSystem.PlayerTurnsLeft--;

    }
}
