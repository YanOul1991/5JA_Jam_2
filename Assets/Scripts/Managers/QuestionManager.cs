using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    public GameManager gameManager;

    public void QuestionColorRed()
    {
        int totalRed = 0;

        foreach (var item in gameManager.activeCards)
        {
            if (item.symbol == Symbol.Heart || item.symbol == Symbol.Diamonds)
            {
                totalRed++;
            }
        }

        // Optionally do something with totalBlack:
        Debug.Log("Total black cards: " + totalRed);
    }
    public void QuestionColorBlack()
    {
        int totalBlack = 0;

        foreach (var item in gameManager.activeCards)
        {
            if (item.symbol == Symbol.Spades || item.symbol == Symbol.Clubs)
            {
                totalBlack++;
            }
        }

        // Optionally do something with totalBlack:
        Debug.Log("Total black cards: " + totalBlack);
    }

    public void QuestionValue()
    {
        int totalKing = 0;
        int totalQueen = 0;
        int totalJack = 0;
        int totalAce = 0;

        foreach (var item in gameManager.activeCards)
        {
            if (item.value == Value.King)
            {
                totalKing++;
            }
            else if (item.value == Value.Queen)
            {
                totalQueen++;
            }
            else if (item.value == Value.Jack)
            {
                totalJack++;
            }
            else if (item.value == Value.Ace)
            {
                totalAce++;
            }
        }
        
        Debug.Log("Total King cards: " + totalKing);
        Debug.Log("Total Queen cards: " + totalQueen);
        Debug.Log("Total Jack cards: " + totalJack);
        Debug.Log("Total Ace cards: " + totalAce);
    }
    
    public void QuestionSymbol()
    {
        int totalHearts = 0;
        int totalDiamonds = 0;
        int totalSpades = 0;
        int totalClubs = 0;

        foreach (var item in gameManager.activeCards)
        {
            if (item.symbol == Symbol.Heart)
            {
                totalHearts++;
            }
            else if (item.symbol == Symbol.Diamonds)
            {
                totalDiamonds++;
            }
            else if (item.symbol == Symbol.Spades)
            {
                totalSpades++;
            }
            else if (item.symbol == Symbol.Clubs)
            {
                totalClubs++;
            }
        }

        Debug.Log("Total Heart cards: " + totalHearts);
        Debug.Log("Total Diamonds cards: " + totalDiamonds);
        Debug.Log("Total Spades cards: " + totalSpades);
        Debug.Log("Total Clubs cards: " + totalClubs);
    }
}
