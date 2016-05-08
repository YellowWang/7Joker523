using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HumanPlayer : Player
{
    public override void AddCard(Card card)
    {
        base.AddCard(card);
        card.Turn();
    }

    public override void MyTurn(List<Card> hand)
    {
        if (CurrentRoundPassed)
        {
            if (TurnFinished != null)
            {
                TurnFinished(this, new List<Card>());
            }
        }
    }
}
