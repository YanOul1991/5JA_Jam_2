using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    List<Card> cards;
    public List<Card> activeCards;
    List<Card> shownCards;
    List<Card> hiddenCards;

    public GameObject cardPrefab;
    public Transform cardParent;
    
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

        // foreach (var item in cards)
        // {
        //    Debug.Log($"{item.value} - {item.symbol}");
        // }

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

        // DisplayAllCards();
    }

    void Update()
    {

    }

    void DisplayAllCards()
    {
        for (int i = 0; i < activeCards.Count; i++)
        {
            Card card = activeCards[i];

            // Create the GameObject for each active card
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            cardObj.name = $"{card.value}_{card.symbol}";

            // Simple positioning (3x3 grid)
            float x = (i % 3) * 2.5f;
            float y = (i / 3) * -3.5f;
            cardObj.transform.position = new Vector3(x, y, 0);

            // Is this card one of the shown ones?
            bool isShown = shownCards.Contains(card);

            // Set up display
            CardDisplay display = cardObj.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.SetCard(card, isShown);
            }
        }
    }
}
