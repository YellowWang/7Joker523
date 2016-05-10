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
    public delegate void MoveDoneEventHandler(Card card);
    public event MoveDoneEventHandler MoveDone;
    // Card move speed in percentage of distance between current position
    // and destination per second
    public float MoveSpeed = 50.0f;
    // Pretty much like MoveSpeed
    public float RotateSpeed = 10.0f;

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

    IEnumerator MoveTo(Vector3[] args)
    {
        float lastTime = Time.time;
        while (transform.position != args[1])
        {
            float percentage = MoveSpeed * (Time.time - lastTime);
            if (Mathf.Abs(percentage - 1.0f) < Mathf.Epsilon)
            {
                percentage = 1.0f;
            }
            transform.position = Vector3.Lerp(args[0], args[1], percentage);
            yield return null;
        }

        if (MoveDone != null)
        {
            MoveDone(this);
        }
    }

    public void StartMoveTo(Vector3 position)
    {
        var args = new Vector3[2] { transform.position, position };
        StartCoroutine(MoveTo(args));
    }
}
