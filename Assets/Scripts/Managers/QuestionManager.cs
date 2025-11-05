using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    public GameManager gameManager;

    [Header("Resultat Indices Couleurs")]
    public TMP_Text resultatIndiceRed;
    public TMP_Text resultatIndiceBlack;

    [Header("Resultat Indices Figures")]
    public TMP_Text resultatIndiceKings;
    public TMP_Text resultatIndiceQueens;
    public TMP_Text resultatIndiceJacks;
    public TMP_Text resultatIndiceAces; 

    [Header("Resultat Indices Symboles")]
    public TMP_Text resultatIndiceHearts;
    public TMP_Text resultatIndiceDiamonds;
    public TMP_Text resultatIndiceSpades;
    public TMP_Text resultatIndiceClubs;

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
        resultatIndiceRed.text = totalRed.ToString();
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
        resultatIndiceBlack.text = totalBlack.ToString();
    }

    public void QuestionValue(string FigureButtonName)
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
        if(FigureButtonName == "IndicesKings")
        {
            Debug.Log("Total King cards: " + totalKing);
            resultatIndiceKings.text = totalKing.ToString();
        }
        else if(FigureButtonName == "IndicesQueens")
        {
            Debug.Log("Total Queen cards: " + totalQueen);
            resultatIndiceQueens.text = totalQueen.ToString();
        }
        else if(FigureButtonName == "IndicesJacks")
        {
            Debug.Log("Total Jack cards: " + totalJack);
            resultatIndiceJacks.text = totalJack.ToString();
        }
        else if(FigureButtonName == "IndicesAces")
        {
            Debug.Log("Total Ace cards: " + totalAce);
            resultatIndiceAces.text = totalAce.ToString();
        }
    }
    
    public void QuestionSymbol(string SymbolButtonName)
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

        if(SymbolButtonName == "IndicesHearts")
        {
            Debug.Log("Total Heart cards: " + totalHearts);
            resultatIndiceHearts.text = totalHearts.ToString();
        }
        else if(SymbolButtonName == "IndicesDiamonds")
        {
            Debug.Log("Total Diamonds cards: " + totalDiamonds);
            resultatIndiceDiamonds.text = totalDiamonds.ToString();
        }
        else if(SymbolButtonName == "IndicesSpades")
        {
            Debug.Log("Total Spades cards: " + totalSpades);
            resultatIndiceSpades.text = totalSpades.ToString();
        }
        else if(SymbolButtonName == "IndicesClubs")
        {
            Debug.Log("Total Clubs cards: " + totalClubs);
            resultatIndiceClubs.text = totalClubs.ToString();
        }
    }
}
