using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {

    public List<Card> Cards = new List<Card>();
    public List<Card> CurrentOutCard = new List<Card>();
    public List<Card> AllOutCardThisRound = new List<Card>();
    public Player[] Players;
    // CurPlayer is the last player who put out card
    private int LastOutCardPlayer;
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
        // Register event handler
        for (int i = 0; i < PlayerGameObjects.Length; ++i)
        {
            Players[i] = PlayerGameObjects[i].GetComponent<Player>();
            Players[i].TurnFinished += new Player.FinishedTurnEventHandler(OnPlayerFinishedTurn);
            Players[i].Idx = i;
        }

        // Deside cards depth
        AdjustCardsDepth();

        // Shuffle the cards
        Shuffle();

        // Deal the first 5 cards to each player
        foreach (var player in Players)
        {
            while (player.NeedMoreCard())
            {
                Deal(player);
            }
        }

        // Start
        StartGame();
    }
	
    int GetCardsPoints(List<Card> cards)
    {
        int points = 0;
        foreach (var card in cards)
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

        return points;
    }

    int NextPlayer(int playerIdx)
    {
        return (playerIdx + 1) % Players.Length;
    }


    void OnPlayerFinishedTurn(Player player, List<Card> outCards)
    {
        // If hand have card
        if (outCards.Count > 0)
        {
            // Record all out cards
            AllOutCardThisRound.AddRange(outCards);

            // Record current out cards, following players will select cards base on this
            CurrentOutCard.Clear();
            CurrentOutCard.AddRange(outCards);

            // Card dont belong to that player anymore
            foreach (var card in outCards)
            {
                card.player = null;
            }

            // Set last hand player
            LastOutCardPlayer = player.Idx;

            // Add current round total points
            points += GetCardsPoints(outCards);
        }

        // Next one's turn
        CurrentTurnPlayer = NextPlayer(CurrentTurnPlayer);
        // If nobody put out cards after this player, then he/she is the winner
        if (CurrentTurnPlayer == LastOutCardPlayer)
        {
            // This round's points go to the winner
            Players[LastOutCardPlayer].points += points;

            // If the all the cards are dealt, the game ends
            if (Cards.Count == 0)
            {
                return;
            }

            // Another round
            StartAnotherRound();

            return;
        }
        else
        {
            Players[CurrentTurnPlayer].MyTurn(CurrentOutCard);
        }
    }

    void StartAnotherRound()
    {
        // Destroy all used cards
        CurrentOutCard.Clear();
        foreach (var card in AllOutCardThisRound)
        {
            Destroy(card.gameObject);
        }
        AllOutCardThisRound.Clear();

        // Reset round points
        points = 0;

        // Deal card to player who don't have enough cards
        var DrawPlayer = LastOutCardPlayer;
        for (int i = 0; i < Players.Length; ++i)
        {
            Players[i].PrepareForNewRound();
            while (Players[DrawPlayer].NeedMoreCard())
            {
                Deal(Players[DrawPlayer]);
            }
            DrawPlayer = NextPlayer(DrawPlayer);
        }

        // Start with last round winner 
        Players[CurrentTurnPlayer].MyTurn(CurrentOutCard);
    }

    void StartGame()
    {
        CurrentTurnPlayer = UnityEngine.Random.Range(0, Players.Length);
        Players[CurrentTurnPlayer].MyTurn(CurrentOutCard);
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
        card.player = player as HumanPlayer;
        player.AddCard(card);
    }
}
