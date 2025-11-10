using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;
using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    static public UIManager Singleton;

    [Header("Bouton Popup Indices")]
    public GameObject indices;
    public TMP_Text nombreIndicesRestantsPopup;
    public TMP_Text nombreIndicesRestants;
    public int nombreIndicesRestantsInt = 3;

    [Header("Sprites Verrouillés (Locked)")]
    public Sprite indicesLockedMediumSprite;
    public Sprite indicesLockedSmallSprite;

    [Header("Indices Figures")]
    public GameObject indicesAces;
    public GameObject indiceKing;
    public GameObject indiceQueens;
    public GameObject indiceJacks;

    [Header("Text Figures Indices Resultats")]
    public GameObject indicesResultAcesText;
    public GameObject indicesResultKingsText;
    public GameObject indicesResultQueensText;
    public GameObject indicesResultJacksText;

    [Header("Sprites Figures Indices Resultats")]
    public Sprite indicesResultAces;
    public Sprite indicesResultKings;
    public Sprite indicesResultQueens;
    public Sprite indicesResultJacks;

    [Header("Indices Symboles")]
    public GameObject indicesHearts;
    public GameObject indiceDiamonds;
    public GameObject indiceSpades;
    public GameObject indiceClubs;

    [Header("Text Symboles Indices Resultats")]
    public GameObject indicesResultHeartsText;
    public GameObject indicesResultDiamondsText;
    public GameObject indicesResultSpadesText;
    public GameObject indicesResultClubsText;

    [Header("Sprites Symboles Indices Resultats")]
    public Sprite indicesResultHearts;
    public Sprite indicesResultDiamonds;
    public Sprite indicesResultSpades;
    public Sprite indicesResultClubs;

    [Header("Indices Couleurs")]
    public GameObject indicesRed;
    public GameObject indicesBlack;

    [Header("Text Couleurs Indices Resultats")]
    public GameObject indicesResultRedText;
    public GameObject indicesResultBlackText;

    [Header("Sprites Couleurs Indices Resultats")]
    public Sprite indicesResultRed;
    public Sprite indicesResultBlack;

    [Header("Boutons Indices")]
    public GameObject indicesCouleursButton;
    public GameObject indicesSymbolesButton;
    public GameObject indicesFiguresButton;

    [Header("Close options")]
    public GameObject closeIndicesBackground;
    public GameObject closeIndicesButton;

    [Header("Sprites Original Indices Figures")]
    public Sprite indicesOriginalAces;
    public Sprite indicesOriginalKings;
    public Sprite indicesOriginalQueens;
    public Sprite indicesOriginalJacks;

    [Header("Sprites Original Indices Symbol")]
    public Sprite indicesOriginalHeart;
    public Sprite indicesOriginalDiamonds;
    public Sprite indicesOriginalSpades;
    public Sprite indicesOriginalClubs;

    [Header("Sprites Original Indices Couleur")]
    public Sprite indicesOriginalRed;
    public Sprite indicesOriginalBlack;


    public QuestionManager questionManager;


    // ------------------------
    // Generic Helper Functions
    // ------------------------

    private void SetIndice(GameObject target, Sprite resultSprite, GameObject resultText)
    {
        Image img = target.GetComponent<Image>();
        img.sprite = resultSprite;
        resultText.SetActive(true);
    }

    private void LockButton(GameObject buttonObj, Sprite lockedSprite)
    {
        Button btn = buttonObj.GetComponent<Button>();
        if (btn != null) btn.interactable = false;

        Image img = buttonObj.GetComponent<Image>();
        if (img != null) img.sprite = lockedSprite;
    }


    // ------------------------
    // UI Visibility
    // ------------------------

    public void ShowIndices() => ShowBoth();
    public void HideIndices() => HideBoth();

    private void ShowBoth()
    {
        indices.SetActive(true);
        closeIndicesBackground.SetActive(true);
    }

    private void HideBoth()
    {
        indices.SetActive(false);
        closeIndicesBackground.SetActive(false);
    }


    // ------------------------
    // COULEURS
    // ------------------------

    public void IndiceCouleur()
    {
        indicesRed.SetActive(true);
        indicesBlack.SetActive(true);
        indicesCouleursButton.SetActive(false);
    }

    public void IndiceRed()
    {
        LockButton(indicesBlack, indicesLockedMediumSprite);
        LockButton(indicesRed, indicesLockedMediumSprite);
        SetIndice(indicesRed, indicesResultRed, indicesResultRedText);

        nombreIndicesRestantsInt--;
        nombreIndicesRestantsPopup.text = nombreIndicesRestantsInt.ToString();
        nombreIndicesRestants.text = nombreIndicesRestantsInt.ToString();
    }

    public void IndiceBlack()
    {
        LockButton(indicesBlack, indicesLockedMediumSprite);
        LockButton(indicesRed, indicesLockedMediumSprite);
        SetIndice(indicesBlack, indicesResultBlack, indicesResultBlackText);

        nombreIndicesRestantsInt--;
        nombreIndicesRestantsPopup.text = nombreIndicesRestantsInt.ToString();
        nombreIndicesRestants.text = nombreIndicesRestantsInt.ToString();
    }


    // ------------------------
    // SYMBOLES
    // ------------------------

    public void IndicesSymboles()
    {
        indicesHearts.SetActive(true);
        indiceDiamonds.SetActive(true);
        indiceSpades.SetActive(true);
        indiceClubs.SetActive(true);

        indicesSymbolesButton.SetActive(false);
    }

    public void IndicesSymbolesSpecific()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (clickedButton == null) return;

        // Lock all others
        BlockIndicesSymboles();

        // Apply result to clicked one
        if (clickedButton == indicesHearts)
            SetIndice(indicesHearts, indicesResultHearts, indicesResultHeartsText);
        else if (clickedButton == indiceDiamonds)
            SetIndice(indiceDiamonds, indicesResultDiamonds, indicesResultDiamondsText);
        else if (clickedButton == indiceSpades)
            SetIndice(indiceSpades, indicesResultSpades, indicesResultSpadesText);
        else if (clickedButton == indiceClubs)
            SetIndice(indiceClubs, indicesResultClubs, indicesResultClubsText);

        nombreIndicesRestantsInt--;
        nombreIndicesRestantsPopup.text = nombreIndicesRestantsInt.ToString();
        nombreIndicesRestants.text = nombreIndicesRestantsInt.ToString();
    }

    private void BlockIndicesSymboles()
    {
        LockButton(indicesHearts, indicesLockedSmallSprite);
        LockButton(indiceDiamonds, indicesLockedSmallSprite);
        LockButton(indiceSpades, indicesLockedSmallSprite);
        LockButton(indiceClubs, indicesLockedSmallSprite);
    }


    // ------------------------
    // FIGURES
    // ------------------------

    public void IndicesFigures()
    {
        indicesAces.SetActive(true);
        indiceKing.SetActive(true);
        indiceQueens.SetActive(true);
        indiceJacks.SetActive(true);

        indicesFiguresButton.SetActive(false);
    }

    public void IndicesFiguresSpecific()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        if (clickedButton == null) return;

        BlockIndicesFigures();

        if (clickedButton == indicesAces)
            SetIndice(indicesAces, indicesResultAces, indicesResultAcesText);
        else if (clickedButton == indiceKing)
            SetIndice(indiceKing, indicesResultKings, indicesResultKingsText);
        else if (clickedButton == indiceQueens)
            SetIndice(indiceQueens, indicesResultQueens, indicesResultQueensText);
        else if (clickedButton == indiceJacks)
            SetIndice(indiceJacks, indicesResultJacks, indicesResultJacksText);

        nombreIndicesRestantsInt--;
        nombreIndicesRestantsPopup.text = nombreIndicesRestantsInt.ToString();
        nombreIndicesRestants.text = nombreIndicesRestantsInt.ToString();
    }

    private void BlockIndicesFigures()
    {
        LockButton(indicesAces, indicesLockedSmallSprite);
        LockButton(indiceKing, indicesLockedSmallSprite);
        LockButton(indiceQueens, indicesLockedSmallSprite);
        LockButton(indiceJacks, indicesLockedSmallSprite);
    }

    public void ResetAllIndices()
    {
        // Reset number of hints
        nombreIndicesRestantsInt = 3;
        nombreIndicesRestantsPopup.text = nombreIndicesRestantsInt.ToString();
        nombreIndicesRestants.text = nombreIndicesRestantsInt.ToString();

        // Hide all result texts
        indicesResultAcesText.SetActive(false);
        indicesResultKingsText.SetActive(false);
        indicesResultQueensText.SetActive(false);
        indicesResultJacksText.SetActive(false);

        indicesResultHeartsText.SetActive(false);
        indicesResultDiamondsText.SetActive(false);
        indicesResultSpadesText.SetActive(false);
        indicesResultClubsText.SetActive(false);

        indicesResultRedText.SetActive(false);
        indicesResultBlackText.SetActive(false);

        // Reactivate all buttons

        //ResetButton(indicesCouleursButton);
        //ResetButton(indicesSymbolesButton);
        //ResetButton(indicesFiguresButton);

        // Re-enable all individual indice buttons
        ResetButton(indicesRed, indicesOriginalRed);
        ResetButton(indicesBlack, indicesOriginalBlack);
        ResetButton(indicesHearts, indicesOriginalHeart);
        ResetButton(indiceDiamonds, indicesOriginalDiamonds);
        ResetButton(indiceSpades, indicesOriginalSpades);
        ResetButton(indiceClubs, indicesOriginalClubs);
        ResetButton(indicesAces, indicesOriginalAces);
        ResetButton(indiceKing, indicesOriginalKings);
        ResetButton(indiceQueens, indicesOriginalQueens);
        ResetButton(indiceJacks, indicesOriginalJacks);

        // Hide all indice subcategories
        indicesRed.SetActive(false);
        indicesBlack.SetActive(false);
        indicesHearts.SetActive(false);
        indiceDiamonds.SetActive(false);
        indiceSpades.SetActive(false);
        indiceClubs.SetActive(false);
        indicesAces.SetActive(false);
        indiceKing.SetActive(false);
        indiceQueens.SetActive(false);
        indiceJacks.SetActive(false);

        // Show main indice buttons
        indicesCouleursButton.SetActive(true);
        indicesSymbolesButton.SetActive(true);
        indicesFiguresButton.SetActive(true);
    }

    // Helper to re-enable button and restore default sprite
    private void ResetButton(GameObject buttonObj, Sprite resetImage)
    {
        if (buttonObj == null) return;

        Button btn = buttonObj.GetComponent<Button>();
        if (btn != null) btn.interactable = true;

        Image img = buttonObj.GetComponent<Image>();
        if (img != null)
        {   
            img.sprite = resetImage;
        }
    }
}
