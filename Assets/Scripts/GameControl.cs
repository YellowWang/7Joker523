using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {

    public List<Card> Cards = new List<Card>();
    public List<Card> CurrentHand = new List<Card>();
    public List<Card> RoundAllHand = new List<Card>();
    public Player[] Players;
    // CurPlayer is the last player who showed hand
    private int LastHandPlayer;
    private int CurrentTurnPlayer;
    private int points = 0;

	// Use this for initialization
	void Start()
    {
        // Find all cards 
        var CardGameObjects = GameObject.FindGameObjectsWithTag("Card");
        Cards = new List<Card>();
        foreach (var i in CardGameObjects)
        {
            Cards.Add(i.GetComponent<Card>());
        }

        // Find all players
        var PlayerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        Players = new Player[PlayerGameObjects.Length];
        for (int i = 0; i < PlayerGameObjects.Length; ++i)
        {
            Players[i] = PlayerGameObjects[i].GetComponent<Player>();
            Players[i].TurnFinished += new Player.FinishedTurnEventHandler(OnPlayerFinishedTurn);
            Players[i].Idx = i;
        }
        Debug.Log("PlayerCount = " + Players.Length);

        // Deside cards depth
        AdjustCardsDepth();

        // Shuffle the cards
        Shuffle();

        // Deal the first 5 cards to each player
        for (int i = 0; i < 5; ++i)
        {
            foreach (var player in Players)
            {
                Deal(player);
            }
        }

        // Start
        StartGame();
    }
	
    int GetHandPoints(List<Card> hand)
    {
        int points = 0;
        if (hand != null)
        {
            foreach (var card in hand)
            {
                // Kind awkward...
                switch (card.rank)
                {
                    case 17:
                        points += 5;
                        break;
                    case 10:
                    case 13:
                        points += 10;
                        break;
                }
            }
        }

        return points;
    }

    void OnPlayerFinishedTurn(Player player, List<Card> hand)
    {
        // If hand have card
        if (hand.Count > 0)
        {
            // Record all hand cards
            RoundAllHand.AddRange(hand);

            // Record current hand
            CurrentHand.AddRange(hand);

            // Card dont belone to that player anymore
            foreach (var card in hand)
            {
                card.player = null;
            }

            // Set last hand player
            LastHandPlayer = player.Idx;

            // Add points
            points += GetHandPoints(hand);
        }

        // Next one's turn
        CurrentTurnPlayer = (CurrentTurnPlayer + 1) % Players.Length;

        if (CurrentTurnPlayer == LastHandPlayer)
        {
            // Calc points
            Players[LastHandPlayer].points += points;

            // Another round
            StartAnotherRound();

            return;
        }

        Players[CurrentTurnPlayer].MyTurn(CurrentHand);
    }

    void StartAnotherRound()
    {
        // Destroy all used cards
        CurrentHand.Clear();
        foreach (var card in RoundAllHand)
        {
            Destroy(card.gameObject);
        }
        RoundAllHand.Clear();

        // Reset points
        points = 0;

        // Deal card to player who don't have enough cards
        var DrawPlayer = LastHandPlayer;
        for (int i = 0; i < Players.Length; ++i)
        {
            Players[i].PrepareForNewRound();
            while (Players[DrawPlayer].NeedMoreCard())
            {
                Deal(Players[DrawPlayer]);
            }
            DrawPlayer = (DrawPlayer + 1) % Players.Length;
        }

        // Start with last hand player 
        Players[CurrentTurnPlayer].MyTurn(CurrentHand);
    }

    void StartGame()
    {
        CurrentTurnPlayer = UnityEngine.Random.Range(0, Players.Length);
        Players[CurrentTurnPlayer].MyTurn(CurrentHand);
    }

    void AdjustCardsDepth()
    {
        Cards.Sort();
        for (int i = 0; i < Cards.Count; ++i)
        {
            Cards[i].transform.SetSiblingIndex(i);
        }
    }

    void Shuffle()
    {
        for (int i = 1; i < Cards.Count; ++i)
        {
            int j = UnityEngine.Random.Range(0, i);
            if (j == i)
            {
                continue;
            }
            else
            {
                var tmp = Cards[i];
                Cards[i] = Cards[j];
                Cards[j] = tmp;
            }
        }
    }

    void Deal(Player player)
    {
        var card = Cards[0];
        Cards.RemoveAt(0);
        card.player = player;
        player.AddCard(card);
    }
}
