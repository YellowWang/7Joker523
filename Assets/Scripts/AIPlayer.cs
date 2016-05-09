using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : Player {
    public override void MyTurn(List<Card> currentOutCards)
    {
        if (CurrentRoundPassed)
        {
            if (TurnFinished != null)
            {
                TurnFinished(this, new List<Card>());
            }
        }

        var myOutCards = new List<Card>();
        if (currentOutCards.Count == 0)
        {
            myOutCards.Add(Cards[0]);
            for (int i = 1; i < Cards.Count; ++i)
            {
                if (Cards[i].rank == myOutCards[0].rank)
                {
                    myOutCards.Add(Cards[i]);
                }
                else
                {
                    break;
                }
            }
            Cards.RemoveRange(0, myOutCards.Count);
        }
        else
        {
            for (int i = 0; i < Cards.Count; ++i)
            {
                if (Cards[i] > currentOutCards[currentOutCards.Count - 1])
                {
                    int count = 1;
                    for (int j = i + 1; j < Cards.Count; ++j)
                    {
                        if (Cards[i].rank == Cards[j].rank)
                        {
                            count++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (count >= currentOutCards.Count)
                    {
                        for (int k = 0; k < currentOutCards.Count; ++k)
                        {
                            myOutCards.Add(Cards[i + k]);
                        }
                        Cards.RemoveRange(i, currentOutCards.Count);
                        break;
                    }
                }

                
            }
        }
        
        // No out cards means passed this round
        if (myOutCards.Count == 0)
        {
            CurrentRoundPassed = true;
        }
        else
        {
            ThisRoundOutCards.AddRange(myOutCards);
        }
        TurnFinished(this, myOutCards);
    }
}
