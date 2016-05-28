using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : Player {
    public override void MyTurn(List<Card> opponentCards)
    {
        base.MyTurn(opponentCards);

        if (CurrentRoundPassed)
        {
            if (TurnFinished != null)
            {
                TurnFinished(this, new List<Card>());
            }
        }

        var outCards = new List<Card>();
        // I am the first one put out cards
        if (opponentCards.Count == 0)
        {
            outCards.Add(Cards[0]);
            Cards.Remove(Cards[0]);
            for (int i = 1; i < Cards.Count; ++i)
            {
                if (Cards[i].rank == outCards[0].rank)
                {
                    outCards.Add(Cards[i]);
                    Cards.Remove(Cards[i]);
                }
                else
                {
                    break;
                }
            }
            
        }
        // I need to anwser others' cards
        else
        {
            for (int i = 0; i < Cards.Count; ++i)
            {
                if (Cards[i] > opponentCards[opponentCards.Count - 1])
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

                    if (count >= opponentCards.Count)
                    {
                        for (int k = 0; k < opponentCards.Count; ++k)
                        {
                            outCards.Add(Cards[i + k]);
                            Cards.Remove(Cards[i + k]);
                        }
                        break;
                    }
                }

                
            }
        }

        PutOutCards(outCards);
        RearrangeCards();

        // No out cards means passed this round
        if (outCards.Count == 0)
        {
            CurrentRoundPassed = true;
        }

        TurnFinished(this, outCards);
    }
}
