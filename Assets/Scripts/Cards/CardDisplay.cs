using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    public GameObject frontFace; // The visible card front (text or image)
    public GameObject backFace;  // The card back

    private Card cardData;

    public void SetCard(Card card, bool isShown)
    {
        cardData = card;
        UpdateVisual(isShown);
    }

    public void UpdateVisual(bool isShown)
    {
        frontFace.SetActive(isShown);
        backFace.SetActive(!isShown);
    }
}
