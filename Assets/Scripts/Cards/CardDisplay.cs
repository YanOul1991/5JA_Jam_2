using Unity.Netcode;
using UnityEngine;

public class CardDisplay : NetworkBehaviour
{
    public GameObject frontFace; // The visible card front (text or image)
    public GameObject backFace;  // The card back

    public SpriteRenderer cardSymbol; 
    public SpriteRenderer cardFigure; 

    public Sprite AceSprite;
    public Sprite KingSprite;
    public Sprite QueenSprite;
    public Sprite JackSprite;

    public Sprite HeartSprite;
    public Sprite DiamondSprite;
    public Sprite SpadeSprite;
    public Sprite ClubSprite;
    
    public Card currentCard;
    public bool isShown;

    public void SetCard(Card card, bool isShown)
    {
        currentCard = card;
        this.isShown = isShown;
    }

    [ClientRpc(AllowTargetOverride = true, Delivery = RpcDelivery.Reliable)]
    public void NetUpdateCardDataClientRpc(Symbol symbol, Value value, bool isShown)
    {
        currentCard = new()
        {
            symbol = symbol,
            value = value
        };
        this.isShown = isShown;
        UpdateVisual(isShown);
    }
    
    public void UpdateVisual(bool isShown)
    {
        frontFace.SetActive(isShown);
        backFace.SetActive(!isShown);
        
        // Assign figure (value) sprite
        Sprite figureSprite = GetFigureSprite(currentCard.value);
        if (figureSprite != null && cardFigure != null)
            cardFigure.sprite = figureSprite;

        // Assign suit (symbol) sprite
        Sprite suitSprite = GetSuitSprite(currentCard.symbol);
        if (suitSprite != null && cardSymbol != null)
            cardSymbol.sprite = suitSprite;
    }

    private Sprite GetFigureSprite(Value value)
    {
        switch (value)
        {
            case Value.Ace: return AceSprite;
            case Value.King: return KingSprite;
            case Value.Queen: return QueenSprite;
            case Value.Jack: return JackSprite;
            default: return null;
        }
    }

    private Sprite GetSuitSprite(Symbol symbol)
    {
        switch (symbol)
        {
            case Symbol.Heart: return HeartSprite;
            case Symbol.Diamonds: return DiamondSprite;
            case Symbol.Spades: return SpadeSprite;
            case Symbol.Clubs: return ClubSprite;
            default: return null;
        }
    }
}


