using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public enum ServerMessage : byte
{
  GameStart,
  RoundStart,
  ResultVictory,
  ResultDraw,
  ResultDefeat,
  ClientConnected,
  Wait
}

public class NetworkServer : NetworkBehaviour
{
  static public NetworkServer Singleton;
  private List<ulong> m_lConnectedClients;
  private List<ulong> m_lClientWaitList;
  private Dictionary<ulong, ulong> m_dClientsCardsData;
  private Dictionary<ulong, int> m_dClientsPoints;
  private List<byte> m_lPickedCards;
  private readonly NetworkMatchmaking m_matchmaking = new();
  private bool m_isMatchmakingRunning = false;

  private void Awake()
  {
    if (Singleton == null)
    {
      Singleton = this;
      DontDestroyOnLoad(gameObject);
    }
    else Destroy(gameObject);

    m_lConnectedClients = new List<ulong>();
  }

  public override void OnNetworkSpawn()
  {
    m_lConnectedClients = new();

    base.OnNetworkSpawn();
    SceneData.Singleton.buttonStart.gameObject.SetActive(false);
    SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
    SceneData.Singleton.textGameStatus.text = $"En attente de joueurs {NetworkManager.Singleton.ConnectedClientsList.Count}/4";

    if (IsServer)
    {
      NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkClientConnected;
      NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkClientDisconnected;
    }
  }

  public override void OnNetworkDespawn()
  {
    base.OnNetworkDespawn();
    NetworkManager.Singleton.OnClientConnectedCallback -= OnNetworkClientConnected;
    NetworkManager.Singleton.OnClientDisconnectCallback -= OnNetworkClientDisconnected;

    SceneData.Singleton.buttonStart.gameObject.SetActive(true);
    SceneData.Singleton.textGameStatus.gameObject.SetActive(false);
  }
  
  private void OnNetworkClientConnected(ulong id)
  {
    Debug.Log($"<color=orange>Connected Clients count {NetworkManager.Singleton.ConnectedClientsList.Count}</color>");
    SceneData.Singleton.textGameStatus.text = $"En attente de joueurs {NetworkManager.Singleton.ConnectedClientsList.Count}/4";
    if (IsServer)
    {
      m_lConnectedClients.Add(id);

      if (m_lConnectedClients.Count >= 2)
      {
        SceneData.Singleton.textGameStatus.gameObject.SetActive(false);
        NetSendMessageToClientRpc((byte)ServerMessage.GameStart);
        RoundStart();
      }
    }
  }

  private void OnNetworkClientDisconnected(ulong id)
  {
    if (!IsServer) return;
    Debug.Log($"The client {id} has left the game");
    m_lConnectedClients.Remove(id);
  }

  private void RoundStart()
  {
    if (!IsServer) return;

    m_dClientsCardsData = new Dictionary<ulong, ulong>();
    m_dClientsPoints = new Dictionary<ulong, int>();
    m_lClientWaitList = new List<ulong>(m_lConnectedClients);
    m_lPickedCards = new List<byte>();

    foreach (ulong client in m_lConnectedClients)
      m_dClientsCardsData.Add(client, 0);

    foreach (Card card in SceneData.Singleton.gameManager.activeCards)
      m_lPickedCards.Add((byte)((byte)card.value | ((byte)card.symbol) << 4));

    NetSendMessageToClientRpc((byte)ServerMessage.GameStart);
    NetSendMessageToClientRpc((byte)ServerMessage.RoundStart);
  }

  [Rpc(SendTo.Server)]
  public void NetSendDataToServerRpc(ulong data, RpcParams rpcParams = default)
  {
    if (!IsServer) return;

    ulong sender = rpcParams.Receive.SenderClientId;
    m_dClientsCardsData[sender] = data;
    m_lClientWaitList.Remove(sender);

    NetSendMessageToClientRpc((byte)ServerMessage.Wait, new ClientRpcParams
    {
      Send = new ClientRpcSendParams { TargetClientIds = new[] { sender } }
    });

    int points = 0;
    List<byte> leftCards = new(m_lPickedCards);

    for (int i = 0; i < 5; i++)
    {
      if (leftCards.Contains((byte)(data & 0xFFul)))
      {
        points++;
        leftCards.Remove((byte)(data & 0xFFul));
      }
      data >>= 8;
    }

    m_dClientsPoints[sender] = points;

#if UNITY_EDITOR
    Debug.Log($"<color=green>The Player {sender} has guessed {points} card(s) correctly!");
#endif

    if (m_lClientWaitList.Count <= 0)
    {
      ulong[] _winner = m_dClientsPoints
        .Where(kvp => kvp.Value == m_dClientsPoints.Values.Max())
        .Select(kvp => kvp.Key)
        .ToArray();

      ulong[] _looser = m_dClientsPoints
        .Where(kvp => kvp.Value < m_dClientsPoints.Values.Max())
        .Select(kvp => kvp.Key)
        .ToArray();

      if (_winner.Length > 1)
      {
        NetSendMessageToClientRpc((byte)ServerMessage.ResultDraw);
      }
      else
      {
        NetSendMessageToClientRpc((byte)ServerMessage.ResultDefeat, new ClientRpcParams
        {
          Send = new ClientRpcSendParams { TargetClientIds = _looser }
        });

        NetSendMessageToClientRpc((byte)ServerMessage.ResultVictory, new ClientRpcParams
        {
          Send = new ClientRpcSendParams { TargetClientIds = _winner }
        });
      }
    }
  }

  [ClientRpc(Delivery = RpcDelivery.Reliable, AllowTargetOverride = true)]
  void NetSendMessageToClientRpc(byte msg, ClientRpcParams rpcParams = default)
  {
    if (!IsClient) return;

    switch ((ServerMessage)msg)
    {
      case ServerMessage.GameStart:
        NetworkPlayer.Singleton.OnGameStart();
        return;
      case ServerMessage.RoundStart:
        SceneData.Singleton.textGameStatus.gameObject.SetActive(false);
        NetworkPlayer.Singleton.OnRoundStart();
        return;
      case ServerMessage.ResultDefeat:
        SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
        SceneData.Singleton.textGameStatus.text = "Partie Perdu";
        Invoke(nameof(GameEnd), 5.0f);
        return;
      case ServerMessage.ResultVictory:
        SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
        SceneData.Singleton.textGameStatus.text = "Partie Gagne!";
        Invoke(nameof(GameEnd), 5.0f);
        return;
      case ServerMessage.ResultDraw:
        SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
        SceneData.Singleton.textGameStatus.text = "Egualite! Recommence un nouveau round";
        Invoke(nameof(RoundStart), 5.0f);
        return;
      case ServerMessage.Wait:
        NetworkPlayer.Singleton.OnRoundEnd();
        return;
      default:
        return;
    }
  }

  private void GameEnd()
  {
    NetworkManager.Singleton.Shutdown();
  }

  public ulong CardsToBytes(ref Card[] cards)
  {
    ulong value = 0;
    for (int i = 0; i < 5; i++) value |= ((ulong)((byte)cards[i].value | (byte)cards[i].symbol << 4)) << (i * 8);
    return value;
  }

  public async void StartMatchmaking()
  {
    if (m_isMatchmakingRunning) return;
    m_isMatchmakingRunning = true;

    try
    {
      SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
      await m_matchmaking.RunMatchmaking();
    }
    catch (System.Exception e)
    {
      Debug.LogError(e.Message);
      throw;
    }
    finally
    {
      m_isMatchmakingRunning = false;      
    }
  }
}
