using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneData : MonoBehaviour
{
  static public SceneData Singleton;
  [SerializeField] public TextMeshProUGUI TextLobbyPlayerCount;
  [SerializeField] public CardSelect m_uiCardSelect;

  void Awake()
  {
    if (Singleton == null) Singleton = this;
    else Destroy(gameObject);
  }

  private void Start()
  {
    m_uiCardSelect.mainObj.SetActive(false);
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
  public Button BtnValueLeft;
  public Button BtnValueRight;
  public Button BtnSymbolLeft;
  public Button BtnSymbolRight;
  public TextMeshProUGUI TextValue;
  public TextMeshProUGUI TextSymbol;
}