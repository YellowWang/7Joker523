using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class Player : MonoBehaviour {

    protected List<Card> Cards = new List<Card>();
    [HideInInspector] public int Idx;
    private List<Card> OpponentHand = new List<Card>();
    private List<Card> LastHand = new List<Card>();
    [HideInInspector] public List<Card> CurHand = new List<Card>();
    public float CardDistance = 30.0f;
    [HideInInspector] public bool CurrentRoundPassed;
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
            Cards[i].transform.position = transform.position;
            Cards[i].transform.rotation = transform.rotation;
            Cards[i].transform.Translate(Cards[i].transform.up * 80, Space.World);
            Cards[i].transform.Translate(Cards[i].transform.right * (i - mid) * CardDistance, Space.World);
        }
    }

    public virtual void NewRound()
    {
        CurrentRoundPassed = false;
    }

    public virtual void MyTurn(List<Card> hand)
    {
        OpponentHand.AddRange(hand);
    }

    public void PickCard(Card picked)
    {
        // Cannot pick a card smaller than opponents hand
        if (OpponentHand.Count > 0 && picked.CompareTo(OpponentHand[0]) < 0)
        {
            return;
        }

        // Already picked card, rank must be the same
        if (CurHand.Count > 0 && CurHand[0].rank != picked.rank)
        {
            return;
        }

        var found = Cards.Find(p => p == picked);
        // Select new card
        if (found != null)
        {
            CurHand.Add(found);
            Cards.Remove(found);
            found.transform.Translate(found.transform.up * 10, Space.World);
        }
        else
        {
            // Unselect selected card
            found = CurHand.Find(p => p == picked);
            if (found != null)
            {
                Cards.Add(found);
                CurHand.Remove(found);
                found.transform.Translate(found.transform.up * -10, Space.World);
            }
        }
    }

    public void ConfirmMyTurn()
    {
        // If have opponent hand, count must be the same
        if (OpponentHand.Count > 0 && CurHand.Count != OpponentHand.Count)
        {
            return;
        }

        CurHand.Sort();
        int mid = Cards.Count / 2;
        for (int i = 0; i < CurHand.Count; ++i)
        {
            CurHand[i].transform.position = transform.position;
            CurHand[i].transform.rotation = transform.rotation;
            CurHand[i].transform.Translate(Cards[i].transform.up * 160, Space.World);
            CurHand[i].transform.Translate(Cards[i].transform.right * (i - mid) * CardDistance, Space.World);
        }

        foreach (var i in LastHand)
        {
            Destroy(i);
        }
        LastHand.Clear();

        LastHand.AddRange(CurHand);
        TurnFinished(this, CurHand);
        CurHand.Clear();
    }

    public void PrepareForNewRound()
    {
        foreach (var i in LastHand)
        {
            Destroy(i);
        }
        LastHand.Clear();
    }

    public bool NeedMoreCard()
    {
        return Cards.Count < 5;
    }
}
