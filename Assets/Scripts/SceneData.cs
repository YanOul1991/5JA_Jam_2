using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneData : MonoBehaviour
{
  static public SceneData Singleton;
  public GameManager gameManager;
  public TextMeshProUGUI textGameStatus;
  public Button buttonStart;
  public Button ButtonSendCards;
  public Button ButtonQuitGame;
  public CardSelect m_uiCardSelect;
  public CardList m_cardList;
  public SerializableDictionnary<Value, Sprite> SpritesValues;
  public SerializableDictionnary<Symbol, Sprite> SpritesSymbols;
  
  void Awake()
  {
    if (Singleton == null) Singleton = this;
    else Destroy(gameObject);
    SpritesValues.Initialize();
    SpritesSymbols.Initialize();
  }

  private void Start()
  {
    m_uiCardSelect.m_mainObj.SetActive(false);
    int listCount = m_cardList.m_transParent.childCount;

    m_cardList.m_cardListItems = new CardListItem[listCount];

    for (int i = 0; i < m_cardList.m_cardListItems.Length; i++)
    {
      GameObject cardObj = m_cardList.m_transParent.GetChild(i).gameObject;

      m_cardList.m_cardListItems[i].m_button = cardObj.GetComponent<Button>();
      m_cardList.m_cardListItems[i].m_imageSymbol = cardObj.transform.GetChild(0).GetComponent<Image>();
      m_cardList.m_cardListItems[i].m_imageValue = cardObj.transform.GetChild(1).GetComponent<Image>();
    }

    for (int i = 0; i < m_cardList.m_cardListItems.Length; i++)
    {
      m_cardList.m_cardListItems[i].m_imageSymbol.sprite = SpritesSymbols[0];
      m_cardList.m_cardListItems[i].m_imageValue.sprite = SpritesValues[0];
    }
  }
}

[Serializable]
public struct CardSelect
{
  public GameObject m_mainObj;
  public Button m_buttonChangeValue;
  public Button m_buttonChangeSymbol;
  public Button m_buttonClose;
  public Image m_imageValue;
  public Image m_imageSymbol;
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
  public Image m_imageSymbol;
  public Image m_imageValue;
}


[Serializable]
public sealed class SerializableDictionnary<TKey, TValue>
{
  [Serializable]
  private class SerializableDictionnaryEntry
  {
    public SerializableDictionnaryEntry(TKey key, TValue value)
    {
      this.key = key;
      this.value = value;
    }

    public TKey key;
    public TValue value;
  }

  [SerializeField] private List<SerializableDictionnaryEntry> _entries = new();
  private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

  public void Initialize()
  {
    foreach (SerializableDictionnaryEntry entry in _entries)
    {
      _dictionary.Add(entry.key, entry.value);
    }
  }

  public TValue this[TKey key]
  {
    get { return _dictionary[key]; }
    set
    {
      _dictionary[key] = value;
      _entries.Add(new SerializableDictionnaryEntry(key, value));
    }
  }

  public bool TryGetValue(TKey key, out TValue value)
  {
    return _dictionary.TryGetValue(key, out value);
  }
}