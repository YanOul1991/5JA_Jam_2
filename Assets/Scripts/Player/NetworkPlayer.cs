using UnityEngine;
using Unity.Netcode;
using UnityEditor.Rendering;

public class NetworkPlayer : NetworkBehaviour
{
  static public NetworkPlayer Singleton;

  private Card m_card;

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
  }

  public void StartNextRound()
  {
    m_card = new Card
    {
      value = Value.King,
      symbol = Symbol.Diamonds
    };

    DisplayCardSelectionUi(true);
  }

  private void DisplayCardSelectionUi(bool _display)
  {
    RefreshCardDisplay();
    if (_display)
    {
      SceneData.Singleton.m_uiCardSelect.mainObj.SetActive(true);
      SceneData.Singleton.m_uiCardSelect.BtnValueLeft.onClick.AddListener(OnCardValueLeftButtonClick);
      SceneData.Singleton.m_uiCardSelect.BtnValueRight.onClick.AddListener(OnCardValueRightButtonClick);
      SceneData.Singleton.m_uiCardSelect.BtnSymbolRight.onClick.AddListener(OnCardSymbolLeftButtonClick);
      SceneData.Singleton.m_uiCardSelect.BtnSymbolLeft.onClick.AddListener(OnCardSymbolRightButtonClick);
    }
    else
    {
      SceneData.Singleton.m_uiCardSelect.mainObj.SetActive(false);
      SceneData.Singleton.m_uiCardSelect.BtnValueLeft.onClick.RemoveAllListeners();
      SceneData.Singleton.m_uiCardSelect.BtnSymbolRight.onClick.RemoveAllListeners();
    }
  }

  private void RefreshCardDisplay()
  {
    SceneData.Singleton.m_uiCardSelect.TextValue.text = m_card.value.ToString();
    SceneData.Singleton.m_uiCardSelect.TextSymbol.text = m_card.symbol.ToString();
  }

  private void OnCardValueLeftButtonClick()
  {
    m_card.value = (int)m_card.value == 0 ? (Value)(int)(Value.Count - 1) : (Value)(int)m_card.value - 1;
    Debug.Log("Value Left Click");
    RefreshCardDisplay();
  }

  private void OnCardValueRightButtonClick()
  {
    m_card.value = (int)m_card.value == (int)(Value.Count - 1) ? (Value)0 : (Value)(int)m_card.value + 1;
    Debug.Log("Value Right Click");
    RefreshCardDisplay();
  }

  private void OnCardSymbolLeftButtonClick()
  {
    m_card.symbol = (int)m_card.symbol == 0 ? (Symbol)(int)(Symbol.Count - 1) : (Symbol)(int)m_card.symbol - 1;
    Debug.Log("Symbol Left Click");
    RefreshCardDisplay();
  }
  
  private void OnCardSymbolRightButtonClick()
  {
    m_card.symbol = (int)m_card.symbol == (int)(Symbol.Count - 1) ? (Symbol)0 : (Symbol)(int)m_card.symbol + 1;
    Debug.Log("Symbol Right Click");
    RefreshCardDisplay();
  }
}