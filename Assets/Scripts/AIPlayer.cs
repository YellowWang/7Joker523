using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : Player {
    public override void MyTurn(List<Card> hand)
    {
        if (CurrentRoundPassed)
        {
            if (TurnFinished != null)
            {
                TurnFinished(this, new List<Card>());
            }
        }

        var myHand = new List<Card>();
        if (hand.Count == 0)
        {
            myHand.Add(Cards[0]);
            for (int i = 1; i < Cards.Count; ++i)
            {
                if (Cards[i].rank == myHand[0].rank)
                {
                    myHand.Add(Cards[i]);
                }
            }
            Cards.RemoveRange(0, myHand.Count);
            TurnFinished(this, myHand);
        }
        else
        {
            for (int i = 0; i < Cards.Count; ++i)
            {
                if (Cards[i] > hand[hand.Count - 1])
                {
                    int count = 1;
                    for (int j = i; j < Cards.Count; ++j)
                    {
                        if (Cards[i].rank == Cards[j].rank)
                        {
                            count++;
                        }
                    }

                    if (count >= hand.Count)
                    {
                        for (int k = 0; k < hand.Count; ++k)
                        {
                            myHand.Add(Cards[i + k]);
                        }
                        Cards.RemoveRange(i, hand.Count);
                    }
                }

                
            }
        }

        TurnFinished(this, myHand);
    }
}
