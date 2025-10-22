using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    List<Card> cards;
    List<Card> activeCards;
    List<Card> shownCards;
    List<Card> hiddenCards;
    
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



        foreach (var item in shownCards)
        {
            Debug.Log($"<color=Green>{item.value} - {item.symbol}</color>");
        }
        foreach (var item in hiddenCards)
        {
            Debug.Log($"<color=Red>{item.value} - {item.symbol}</color>");
        }
    }

    void Update()
    {
        
    }
}
