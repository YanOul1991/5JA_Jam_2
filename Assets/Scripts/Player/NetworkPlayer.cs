using UnityEngine;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;

public class NetworkPlayer : NetworkBehaviour
{
  static public NetworkPlayer Singleton;

  private Card m_cardSelect;

  private Card[] m_cardListData;
  private int m_activeCardIndex;

  private void Awake()
  {
    if (Singleton == null)
    {
      Singleton = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();

    m_cardListData = new Card[5];
  }

  public void StartNextRound()
  {
    m_cardSelect = new Card
    {
      value = 0,
      symbol = 0
    };

    for (int i = 0; i < m_cardListData.Length; i++)
    {
      m_cardListData[i] = new Card
      {
        value = 0,
        symbol = 0
      };
    }

    DisplayCardSelectionUi(true);
    SceneData.Singleton.m_cardList.m_transParent.gameObject.SetActive(true);

    SceneData.Singleton.m_cardList.m_cardListItems[0].m_button.onClick.AddListener(delegate
    {
      ModifySelectedCard(0);
    });
    SceneData.Singleton.m_cardList.m_cardListItems[1].m_button.onClick.AddListener(delegate
    {
      ModifySelectedCard(1);
    });
    SceneData.Singleton.m_cardList.m_cardListItems[2].m_button.onClick.AddListener(delegate
    {
      ModifySelectedCard(2);
    });
    SceneData.Singleton.m_cardList.m_cardListItems[2].m_button.onClick.AddListener(delegate
    {
      ModifySelectedCard(2);
    });
    SceneData.Singleton.m_cardList.m_cardListItems[3].m_button.onClick.AddListener(delegate
    {
      ModifySelectedCard(3);
    });
    SceneData.Singleton.m_cardList.m_cardListItems[4].m_button.onClick.AddListener(delegate
    {
      ModifySelectedCard(4);
    });

    SceneData.Singleton.m_buttonSendCards.onClick.AddListener(OnSendCardsToServer);
  }

  private void DisplayCardSelectionUi(bool _display)
  {
    RefreshCardDisplay();
    if (_display)
    {
      SceneData.Singleton.m_uiCardSelect.mainObj.SetActive(true);
      SceneData.Singleton.m_uiCardSelect.m_buttonValue.onClick.AddListener(OnCardValueButtonClick);
      SceneData.Singleton.m_uiCardSelect.m_buttonSymbol.onClick.AddListener(OnCardSymbolButtonClick);
    }
    else
    {
      SceneData.Singleton.m_uiCardSelect.mainObj.SetActive(false);
      SceneData.Singleton.m_uiCardSelect.m_buttonValue.onClick.RemoveAllListeners();
      SceneData.Singleton.m_uiCardSelect.m_buttonSymbol.onClick.RemoveAllListeners();
    }
  }

  private void RefreshCardDisplay()
  {
    SceneData.Singleton.m_uiCardSelect.TextValue.text = m_cardSelect.value.ToString();
    SceneData.Singleton.m_uiCardSelect.TextSymbol.text = m_cardSelect.symbol.ToString();

    for (int i = 0; i < SceneData.Singleton.m_cardList.m_cardListItems.Length; i++)
    {
      SceneData.Singleton.m_cardList.m_cardListItems[i].m_symbolText.text = m_cardListData[i].symbol.ToString();
      SceneData.Singleton.m_cardList.m_cardListItems[i].m_valueText.text = m_cardListData[i].value.ToString();
    }
  }

  private void ModifySelectedCard(int index)
  {
    Debug.Log($"Modifing {index}");
    m_activeCardIndex = index;

    m_cardSelect.value = m_cardListData[m_activeCardIndex].value;
    m_cardSelect.symbol = m_cardListData[m_activeCardIndex].symbol;
    RefreshCardDisplay();
  }

  private void OnCardValueButtonClick()
  {
    Debug.Log("card Click");
    m_cardListData[m_activeCardIndex].value = (int)m_cardListData[m_activeCardIndex].value == (int)(Value.Count - 1) ? (Value)0 : (Value)(int)m_cardListData[m_activeCardIndex].value + 1;
    m_cardSelect.value = m_cardListData[m_activeCardIndex].value;
    RefreshCardDisplay();
  }

  private void OnCardSymbolButtonClick()
  {
    Debug.Log("card Click");
    m_cardListData[m_activeCardIndex].symbol = (int)m_cardListData[m_activeCardIndex].symbol == (int)(Symbol.Count - 1) ? (Symbol)0 : (Symbol)(int)m_cardListData[m_activeCardIndex].symbol + 1;
    m_cardSelect.symbol = m_cardListData[m_activeCardIndex].symbol;
    RefreshCardDisplay();
  }

  private void OnSendCardsToServer()
  {
    ulong cardData = NetworkServer.Singleton.ConvertCardsToByte(m_cardListData);
    NetworkServer.Singleton.NetSendDataToServerRpc(cardData);
  }
}