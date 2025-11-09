using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    List<Card> cards;
    public List<Card> activeCards;
    public List<Card> shownCards;
    public List<Card> hiddenCards;

    public GameObject cardPrefab;
    public Transform cardParent;

    public Sprite Ace;
    public Sprite King;
    public Sprite Queen;
    public Sprite Jack;
    public Sprite Heart;
    public Sprite Diamonds;
    public Sprite Spades;
    public Sprite Clubs;
    
    void Awake()
    {
        cards = new List<Card>();
        for (int i = 0; i < (int)Value.Count; i++)
        {
            for (int j = 0; j < (int)Symbol.Count; j++)
            {
                //Debug.Log($"{(Value)i} - {(Symbol)j}");
                Card newCard = new Card();
                newCard.value = (Value)i;
                newCard.symbol = (Symbol)j;
                cards.Add(newCard);
            }
        }

        activeCards = new List<Card>();
        for (int i = 0; i < 9; i++)
        {
            int index = Random.Range(0, cards.Count);
            Card cardSelected = cards[index];
            activeCards.Add(cardSelected);
            cards.RemoveAt(index);
        }

        shownCards = new List<Card>();
        hiddenCards = new List<Card>();

        for (int i = 0; i < 9; i++)
        {
            hiddenCards.Add(activeCards[i]);
        }

        for (int i = 0; i < 4; i++)
        {
            int index = Random.Range(0, hiddenCards.Count);
            Card cardSelected = hiddenCards[index];
            shownCards.Add(cardSelected);
            hiddenCards.RemoveAt(index);
        }



        foreach (Card item in shownCards)
        {
            Debug.Log($"<color=Green>{item.value} - {item.symbol}</color>");
        }
        foreach (Card item in hiddenCards)
        {
            Debug.Log($"<color=Red>{item.value} - {item.symbol}</color>");
        }
        foreach (Card item in activeCards)
        {
            Debug.Log($"<color=Purple>{item.value} - {item.symbol}</color>");
        }

        DisplayAllCards();
    }

    void DisplayAllCards()
    {
        for (int i = 0; i < activeCards.Count; i++)
        {
            Card card = activeCards[i];

            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            cardObj.name = $"{card.value}_{card.symbol}";

            float x = (i % 3) * 0.15f;
            float z = (i / 3) * 0.2f;
            cardObj.transform.localPosition = new Vector3(x, 0.1f, z);

            bool isShown = shownCards.Contains(card);

            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            if (display != null)
            {
                // Assign Sprites from GameManager
                display.AceSprite = Ace;
                display.KingSprite = King;
                display.QueenSprite = Queen;
                display.JackSprite = Jack;

                display.HeartSprite = Heart;
                display.DiamondSprite = Diamonds;
                display.SpadeSprite = Spades;
                display.ClubSprite = Clubs;

                // Now show the card
                display.SetCard(card, isShown);
            }
        }
    }
}
