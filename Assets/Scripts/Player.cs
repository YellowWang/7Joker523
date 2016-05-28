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
        Debug.Log(string.Format("Player {0} AddCard {1}", ToString(), card));
        Cards.Add(card);
        card.transform.SetParent(transform);
        RearrangeCards();
    }

    protected void RearrangeCards()
    {
        Cards.Sort();
        int mid = Cards.Count / 2;
        for (int i = 0; i < Cards.Count; ++i)
        {
            var position = transform.position;
            position += Cards[i].transform.up * 80;
            position += Cards[i].transform.right * (i - mid) * CardDistance;
            Cards[i].StartMoveTo(position);
            Cards[i].transform.SetSiblingIndex(i);
        }
    }

    public virtual void NewRound()
    {
        CurrentRoundPassed = false;
    }

    public virtual void MyTurn(List<Card> opponentCards)
    {
        Debug.Log(string.Format("{0}'s turn, opponent cards:", ToString()));
        foreach (var card in opponentCards)
        {
            Debug.Log(card.ToString());
        }
    }

    public void PrepareForNewRound()
    {
        ThisRoundOutCards.Clear();
        CurrentRoundPassed = false;
    }

    public int NeedCards()
    {
        return 5 - Cards.Count;
    }

    public override string ToString()
    {
        return string.Format("Player {0}", name);
    }

    public void PutOutCards(List<Card> outCards)
    {
        outCards.Sort();
        ThisRoundOutCards.AddRange(outCards);
        var outCardsAnchor = transform.Find("OutCardsAnchor");
        for (int i = 0; i < outCards.Count; ++i)
        {
            Debug.Log(string.Format("{0} put out card {1}", ToString(), outCards[i].ToString()));

            outCards[i].transform.position = outCardsAnchor.transform.position;
            outCards[i].transform.rotation = Quaternion.identity;
            outCards[i].transform.Translate(Cards[i].transform.right * (i + ThisRoundOutCards.Count) * CardDistance, Space.World);
            outCards[i].ShowFront();
        }

        for (int i = 0; i < ThisRoundOutCards.Count; ++i)
        {
            ThisRoundOutCards[i].transform.SetSiblingIndex(i);
        }
    }
}
