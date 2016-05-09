using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HumanPlayer : Player
{
    protected List<Card> CurrentOutCards = new List<Card>();
    [HideInInspector] public List<Card> CurrentSelectedCards = new List<Card>();

    public override void AddCard(Card card)
    {
        base.AddCard(card);
        card.ShowFront();
    }

    public override void MyTurn(List<Card> currentOutCards)
    {
        CurrentOutCards.Clear();
        CurrentOutCards.AddRange(currentOutCards);

        if (CurrentRoundPassed)
        {
            if (TurnFinished != null)
            {
                TurnFinished(this, new List<Card>());
            }
        }
    }

    public void PickCard(Card picked)
    {
        // Cannot pick a card smaller than opponents hand
        if (CurrentOutCards.Count > 0 && picked.CompareTo(CurrentOutCards[0]) < 0)
        {
            return;
        }

        // Already picked card, rank must be the same
        if (CurrentSelectedCards.Count > 0 && CurrentSelectedCards[0].rank != picked.rank)
        {
            return;
        }

        var found = Cards.Find(p => p == picked);
        // Select new card
        if (found != null)
        {
            CurrentSelectedCards.Add(found);
            Cards.Remove(found);
            found.transform.Translate(found.transform.up * 10, Space.World);
        }
        else
        {
            // Unselect selected card
            found = CurrentSelectedCards.Find(p => p == picked);
            if (found != null)
            {
                Cards.Add(found);
                CurrentSelectedCards.Remove(found);
                found.transform.Translate(found.transform.up * -10, Space.World);
            }
        }
    }

    public void ConfirmMyTurn()
    {
        // If have opponent hand, count must be the same
        if (CurrentOutCards.Count > 0 && CurrentSelectedCards.Count > 0 && CurrentSelectedCards.Count != CurrentOutCards.Count)
        {
            return;
        }

        CurrentSelectedCards.Sort();
        int mid = Cards.Count / 2;
        for (int i = 0; i < CurrentSelectedCards.Count; ++i)
        {
            CurrentSelectedCards[i].transform.position = transform.position;
            CurrentSelectedCards[i].transform.rotation = transform.rotation;
            CurrentSelectedCards[i].transform.Translate(Cards[i].transform.up * 160, Space.World);
            CurrentSelectedCards[i].transform.Translate(Cards[i].transform.right * (i - mid) * CardDistance, Space.World);
        }

        ThisRoundOutCards.AddRange(CurrentSelectedCards);
        List<Card> outCards = new List<Card>();
        outCards.AddRange(CurrentSelectedCards);
        CurrentSelectedCards.Clear();
        TurnFinished(this, outCards);
    }
}
