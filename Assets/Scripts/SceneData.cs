using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneData : MonoBehaviour
{
  static public SceneData Singleton;
  [SerializeField] public TextMeshProUGUI TextLobbyPlayerCount;
  [SerializeField] public CardSelect m_uiCardSelect;
  public CardList m_cardList;
  public Button m_buttonSendCards;

  void Awake()
  {
    if (Singleton == null) Singleton = this;
    else Destroy(gameObject);
  }

  private void Start()
  {
    m_uiCardSelect.mainObj.SetActive(false);
    int listCount = m_cardList.m_transParent.childCount;

    m_cardList.m_cardListItems = new CardListItem[listCount];

    for (int i = 0; i < m_cardList.m_cardListItems.Length; i++)
    {
      GameObject cardObj = m_cardList.m_transParent.GetChild(i).gameObject;
      
      m_cardList.m_cardListItems[i].m_button = cardObj.GetComponent<Button>();
      m_cardList.m_cardListItems[i].m_symbolText = cardObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
      m_cardList.m_cardListItems[i].m_valueText = cardObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
  }

  public void SetTextLobbyPlayerCount(int count)
  {
    TextLobbyPlayerCount.text = $"Lobby Player Count: {count}";
  }
}

[Serializable]
public struct CardSelect
{
  public GameObject mainObj;
  public Button m_buttonValue;
  public Button m_buttonSymbol;
  public TextMeshProUGUI TextValue;
  public TextMeshProUGUI TextSymbol;
}

[Serializable]
public struct CardList
{
  public RectTransform m_transParent;
  public CardListItem[] m_cardListItems;
}

[Serializable]
public struct CardListItem
{
  public Button m_button;
  public TextMeshProUGUI m_symbolText;
  public TextMeshProUGUI m_valueText;
}