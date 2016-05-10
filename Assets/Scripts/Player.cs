using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class Player : MonoBehaviour
{
    protected List<Card> Cards = new List<Card>();
    [HideInInspector] public int Idx;
    protected List<Card> ThisRoundOutCards = new List<Card>();
    public float CardDistance = 25.0f;
    [HideInInspector] public bool CurrentRoundPassed = false;
    [HideInInspector] public int points = 0;

    public delegate void FinishedTurnEventHandler(Player player, List<Card> hand);
    public FinishedTurnEventHandler TurnFinished;

    public virtual void AddCard(Card card)
    {
        Cards.Add(card);
        RearrangeCards();
    }

    void RearrangeCards()
    {
        Cards.Sort();
        int mid = Cards.Count / 2;
        for (int i = 0; i < Cards.Count; ++i)
        {
            var position = transform.position;
            position += Cards[i].transform.up * 80;
            position += Cards[i].transform.right * (i - mid) * CardDistance;
            Cards[i].StartMoveTo(position);
        }
    }

    public virtual void NewRound()
    {
        CurrentRoundPassed = false;
    }

    abstract public void MyTurn(List<Card> currentOutCards);

    public void PrepareForNewRound()
    {
        ThisRoundOutCards.Clear();
        CurrentRoundPassed = false;
    }

    public int NeedCards()
    {
        return 5 - Cards.Count;
    }
}
