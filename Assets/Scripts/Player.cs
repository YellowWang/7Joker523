﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class Player : MonoBehaviour
{
    protected List<Card> Cards = new List<Card>();
    [HideInInspector] public int Idx;
    protected List<Card> ThisRoundOutCards = new List<Card>();
    public float CardDistance = 30.0f;
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

    abstract public void MyTurn(List<Card> currentOutCards);

    public void PrepareForNewRound()
    {
        ThisRoundOutCards.Clear();
        CurrentRoundPassed = false;
    }

    public bool NeedMoreCard()
    {
        return Cards.Count < 5;
    }
}
