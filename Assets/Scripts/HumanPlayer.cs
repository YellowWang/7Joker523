using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HumanPlayer : Player
{
    protected List<Card> OpponentCards = new List<Card>();
    [HideInInspector] public List<Card> CurrentSelectedCards = new List<Card>();

    public override void AddCard(Card card)
    {
        base.AddCard(card);
        card.ShowFront();
    }

    public override void MyTurn(List<Card> opponentCards)
    {
        base.MyTurn(opponentCards);

        OpponentCards.Clear();
        OpponentCards.AddRange(opponentCards);

        if (CurrentRoundPassed)
        {
            if (TurnFinished != null)
            {
                TurnFinished(this, new List<Card>());
            }
        }
    }

    public void SelectCard(Card card)
    {
        Debug.Log(string.Format("{0} SelectCard {1}", ToString(), card));

        // Cannot pick a card smaller than opponents hand
        if (OpponentCards.Count > 0 && card.CompareTo(OpponentCards[0]) < 0)
        {
            return;
        }

        // Already picked card, rank must be the same
        if (CurrentSelectedCards.Count > 0 && CurrentSelectedCards[0].rank != card.rank)
        {
            return;
        }

        var found = Cards.Find(p => p == card);
        // Select new card
        if (found != null)
        {
            CurrentSelectedCards.Add(found);
            Cards.Remove(found);
            found.transform.Translate(found.transform.up * 10);
        }
        else
        {
            // Unselect selected card
            found = CurrentSelectedCards.Find(p => p == card);
            if (found != null)
            {
                Cards.Add(found);
                CurrentSelectedCards.Remove(found);
                found.transform.Translate(found.transform.up * -10, Space.World);
            }
        }
    }

    public void ConfirmSelection()
    {
        Debug.Log(string.Format("{0} ConfirmSelection", ToString()));

        // If have opponent hand, count must be the same
        if (OpponentCards.Count > 0 && CurrentSelectedCards.Count > 0 && CurrentSelectedCards.Count != OpponentCards.Count)
        {
            return;
        }

        PutOutCards(CurrentSelectedCards);
        RearrangeCards();

        List<Card> outCards = new List<Card>();
        outCards.AddRange(CurrentSelectedCards);
        CurrentSelectedCards.Clear();
        TurnFinished(this, outCards);
    }
}
