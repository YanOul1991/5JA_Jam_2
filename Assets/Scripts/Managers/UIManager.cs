using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour
{
    public Sprite indicesLockedMediumSprite;
    public Sprite indicesLockedSmallSprite;
    public Sprite indicesResultRed;
    public Sprite indicesResultBlack;

    public GameObject indices;
    public GameObject indicesRed;
    public GameObject indicesBlack;

    public GameObject indicesResultRedText;
    public GameObject indicesResultBlackText;

    public GameObject indicesCouleursButton;
    public GameObject indicesSymbolesButton;
    public GameObject indicesFiguresButton;

    public GameObject closeIndicesButton;


    public QuestionManager questionManager;

    // Show the UI
    public void ShowIndices()
    {
        indices.SetActive(true);
    }

    // Hide the UI
    public void HideIndices()
    {
        indices.SetActive(false);
    }

    public void IndiceCouleur()
    {
        indicesRed.SetActive(true);
        indicesBlack.SetActive(true);
        indicesCouleursButton.SetActive(false);
    }

    public void IndiceRed()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
        Debug.Log("You clicked: " + clickedButton.name);
        indicesBlack.gameObject.GetComponent<Button>().interactable = false;
        indicesBlack.gameObject.GetComponent<Image>().sprite = indicesLockedMediumSprite;
        indicesRed.gameObject.GetComponent<Image>().sprite = indicesResultRed;
        indicesResultRedText.SetActive(true);
    }

    public void IndiceBlack()
    {
        indicesRed.gameObject.GetComponent<Button>().interactable = false;
        indicesRed.gameObject.GetComponent<Image>().sprite = indicesLockedMediumSprite;
        indicesBlack.gameObject.GetComponent<Image>().sprite = indicesResultBlack;
        indicesResultBlackText.SetActive(true);
    }

    public void indicesSymboles()
    {
        indicesSymbolesButton.SetActive(false);
    }

    public void IndicesFigures()
    {
        indicesFiguresButton.SetActive(false);
    }
}