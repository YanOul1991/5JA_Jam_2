using UnityEngine;
using Unity.Netcode;
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

    SceneManager.sceneLoaded += (scene, mode) =>
    {
      if (scene.name == "_Test_Yanis")
        SceneData.Singleton.buttonStart.onClick.AddListener(() =>
        {
          Debug.Log("Start Button Clicked!!!");
          NetworkServer.Singleton.StartMatchmaking();
        });
    };
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();

    SceneData.Singleton.ButtonQuitGame.onClick.AddListener(() =>
    {
      NetworkManager.Singleton.Shutdown();
    });
  }

  public override void OnNetworkDespawn()
  {
    base.OnNetworkDespawn();

    SceneData sdata = SceneData.Singleton;

    sdata.ButtonQuitGame.onClick.RemoveAllListeners();

    sdata.m_uiCardSelect.m_mainObj.SetActive(false);
    sdata.m_uiCardSelect.m_buttonClose.onClick.RemoveAllListeners();
    sdata.m_uiCardSelect.m_buttonChangeValue.onClick.RemoveAllListeners();
    sdata.m_uiCardSelect.m_buttonChangeSymbol.onClick.RemoveAllListeners();

    sdata.ButtonSendCards.onClick.RemoveAllListeners();
    sdata.textGameStatus.text = "";
    sdata.textGameStatus.gameObject.SetActive(false);

    sdata.m_cardList.m_transParent.gameObject.SetActive(false);
    SceneData.Singleton.GameObjIndices.SetActive(false);

    Debug.Log("Disconnected from server");
  }

  public void OnGameStart()
  {
    m_cardSelect = new Card();
    m_cardListData = new Card[5];
    for (int i = 0; i < m_cardListData.Length; i++) m_cardListData[i] = new Card();
  }

  public void OnRoundStart()
  {
    SceneData.Singleton.uiManager.ResetAllIndices();
    SceneData.Singleton.GameObjIndices.SetActive(true);

    SceneData.Singleton.m_cardList.m_transParent.gameObject.SetActive(true);

    for (int i = 0; i < SceneData.Singleton.m_cardList.m_cardListItems.Length; i++)
    {
      int index = i;
      SceneData.Singleton.m_cardList.m_cardListItems[index].m_button.onClick.AddListener(() => ModifySelectedCard(index));
    }

    SceneData.Singleton.m_uiCardSelect.m_buttonClose.onClick.AddListener(() => DisplayCardSelectionUi(false));
    SceneData.Singleton.m_uiCardSelect.m_buttonChangeValue.onClick.AddListener(OnCardValueButtonClick);
    SceneData.Singleton.m_uiCardSelect.m_buttonChangeSymbol.onClick.AddListener(OnCardSymbolButtonClick);
    SceneData.Singleton.ButtonSendCards.onClick.AddListener(OnSendCardsToServer);

    RefreshCardDisplay();
  }
  
  public void OnRoundEnd()
  {
    foreach (CardListItem item in SceneData.Singleton.m_cardList.m_cardListItems)
      item.m_button.onClick.RemoveAllListeners();

    SceneData.Singleton.m_uiCardSelect.m_buttonClose.onClick.RemoveAllListeners();
    SceneData.Singleton.m_uiCardSelect.m_buttonChangeValue.onClick.RemoveAllListeners();
    SceneData.Singleton.m_uiCardSelect.m_buttonChangeSymbol.onClick.RemoveAllListeners();

    SceneData.Singleton.ButtonSendCards.onClick.RemoveAllListeners();

    SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
    SceneData.Singleton.textGameStatus.text = "En attente des resultats";

    SceneData.Singleton.m_cardList.m_transParent.gameObject.SetActive(false);
    SceneData.Singleton.m_uiCardSelect.m_mainObj.SetActive(false);
    SceneData.Singleton.GameObjIndices.SetActive(false);
  }

  private void DisplayCardSelectionUi(bool _display)
  {
    RefreshCardDisplay();
    SceneData.Singleton.m_uiCardSelect.m_mainObj.SetActive(_display);
  }

  private void RefreshCardDisplay()
  {
    SceneData.Singleton.m_uiCardSelect.m_imageValue.sprite = SceneData.Singleton.SpritesValues[m_cardSelect.value];
    SceneData.Singleton.m_uiCardSelect.m_imageSymbol.sprite = SceneData.Singleton.SpritesSymbols[m_cardSelect.symbol];

    for (int i = 0; i < SceneData.Singleton.m_cardList.m_cardListItems.Length; i++)
    {
      SceneData.Singleton.m_cardList.m_cardListItems[i].m_imageSymbol.sprite =
        SceneData.Singleton.SpritesSymbols[m_cardListData[i].symbol];
        
      SceneData.Singleton.m_cardList.m_cardListItems[i].m_imageValue.sprite =
        SceneData.Singleton.SpritesValues[m_cardListData[i].value];
    }
  }

  private void ModifySelectedCard(int index)
  {
    m_activeCardIndex = index;
    m_cardSelect.value = m_cardListData[m_activeCardIndex].value;
    m_cardSelect.symbol = m_cardListData[m_activeCardIndex].symbol;
    DisplayCardSelectionUi(true);
    RefreshCardDisplay();
  }

  private void OnCardValueButtonClick()
  {
    m_cardListData[m_activeCardIndex].value =
      (int)m_cardListData[m_activeCardIndex].value == (int)(Value.Count - 1) ? 0 : (Value)(int)m_cardListData[m_activeCardIndex].value + 1;

    m_cardSelect.value = m_cardListData[m_activeCardIndex].value;
    RefreshCardDisplay();
  }

  private void OnCardSymbolButtonClick()
  {
    m_cardListData[m_activeCardIndex].symbol =
      (int)m_cardListData[m_activeCardIndex].symbol == (int)(Symbol.Count - 1) ? 0 : (Symbol)(int)m_cardListData[m_activeCardIndex].symbol + 1;

    m_cardSelect.symbol = m_cardListData[m_activeCardIndex].symbol;
    RefreshCardDisplay();
  }

  private void OnSendCardsToServer()
  {
    ulong cardData = NetworkServer.Singleton.CardsToBytes(ref m_cardListData);
    NetworkServer.Singleton.NetSendDataToServerRpc(cardData);
  }
}