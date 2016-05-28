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
    public delegate void DealDoneEventHandler();
    public event DealDoneEventHandler DealDone;
    public float DealCardInterval = 0.2f;

    void Awake()
    {
        // Turn this off when release
        Debug.logger.logEnabled = true;
    }

	// Use this for initialization
	void Start()
    {
        // Register deal done event
        DealDone += OnDealDone;

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
        //AdjustCardsDepth();

        // Shuffle the cards
        Shuffle();

        // Deal the first 5 cards to each player
        StartCoroutine(InitialDeal());

        // Start with random player
        CurrentTurnPlayer = UnityEngine.Random.Range(0, Players.Length);

        // Deal cards
        InitialDeal();
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
        for (int i = 0; i < Players.Length; ++i)
        {
            Players[i].PrepareForNewRound();
        }

        StartCoroutine(RoundDeal());
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

    // Each one take one card in turns
    IEnumerator InitialDeal()
    {
        int needCards = 0;
        for (int i = 0; i < Players.Length; ++i)
        {
            needCards += Players[i].NeedCards();
        }

        while (needCards > 0)
        {
            for (int i = 0; i < Players.Length; ++i)
            {
                if (Players[i].NeedCards() > 0)
                {
                    var card = Cards[0];
                    Cards.RemoveAt(0);
                    card.player = Players[i] as HumanPlayer;
                    Players[i].AddCard(card);
                    needCards--;
                    yield return new WaitForSeconds(DealCardInterval);
                }
            }
        }

        DealDone();
    }
    
    // Everyone keeps taking cards until got enough
    IEnumerator RoundDeal()
    {
        for (int i = 0; i < Players.Length; ++i)
        {
            while (Players[i].NeedCards() > 0 && Cards.Count > 0)
            {
                var card = Cards[0];
                Cards.RemoveAt(0);
                card.player = Players[i] as HumanPlayer;
                Players[i].AddCard(card);
                yield return new WaitForSeconds(DealCardInterval);
            }
        }

        DealDone();
    }

    void OnDealDone() {
        Players[CurrentTurnPlayer].MyTurn(CurrentOutCard);
    }
}
