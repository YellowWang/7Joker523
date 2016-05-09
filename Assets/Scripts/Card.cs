using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public enum CardSuit
{
    Spade,
    Heart,
    Club,
    Diamond,
}

public class Card : MonoBehaviour, IComparable
{
    private Image image;
    public Sprite front;
    public int rank;
    public CardSuit suit;
    [HideInInspector] public HumanPlayer player;

    // Use this for initialization
    void Awake () {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowFront()
    {
        image.overrideSprite = front;
    }

    public void ShowBack()
    {
        image.overrideSprite = null;
    }

    public int CompareTo(object obj)
    {
        var otherCard = obj as Card;
        if (otherCard == null)
        {
            return 0;
        }
        if (rank > otherCard.rank)
        {
            return 1;
        }
        else if (rank < otherCard.rank)
        {
            return -1;
        }
        else
        {
            if (suit > otherCard.suit)
            {
                return 1;
            }
            else if (suit < otherCard.suit)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    public void Picked()
    {
        if (player != null)
        {
            player.PickCard(this);
        }
    }

    public static bool operator <(Card card1, Card card2)
    {
        return card1.CompareTo(card2) < 0;
    }

    public static bool operator >(Card card1, Card card2)
    {
        return card1.CompareTo(card2) > 0;
    }
}
