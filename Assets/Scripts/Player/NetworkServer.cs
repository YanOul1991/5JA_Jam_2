using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public enum ServerMessage : byte
{
  GameStart,
  RoundEnd,
  RoundStart,
  ResultVictory,
  ResultDefeat,
  ClientConnected
}

public class NetworkServer : NetworkBehaviour
{
  static public NetworkServer Singleton;
  private NetworkManager m_NetWorkManager;
  private List<ulong> m_lConnectedClients;
  private List<ulong> m_lClientWaitList;
  private Dictionary<ulong, ulong> m_dClientsCardsData;

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

    m_lConnectedClients = new List<ulong>();
  }

  private void Start()
  {
    m_NetWorkManager = NetworkManager.Singleton;
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    SceneData.Singleton.TextLobbyPlayerCount.gameObject.SetActive(true);

    if (IsServer) NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkClientConnected;
  }

  public override void OnNetworkDespawn()
  {
    base.OnNetworkDespawn();
  }

  private void OnNetworkClientConnected(ulong id)
  {
    if (IsServer)
    {
      Debug.Log($"<color=green>New Client connected to server.</color>");

      m_lConnectedClients.Add(id);
      NetUpdateClientCountsRpc();

      RoundStart();
      if (m_lConnectedClients.Count >= 3)
      {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnNetworkClientConnected;
        NetSendMessageToClientRpc((byte)ServerMessage.GameStart);
        RoundStart();
        SceneData.Singleton.TextLobbyPlayerCount.gameObject.SetActive(false);
      }
    }
  }

  private void RoundStart()
  {
    if (!IsServer) return;

    m_dClientsCardsData = new Dictionary<ulong, ulong>();
    m_lClientWaitList = new List<ulong>(m_lConnectedClients);

    foreach (ulong client in m_lConnectedClients)
    {
      m_dClientsCardsData.Add(client, 0);
    }
    NetSendMessageToClientRpc((byte)ServerMessage.GameStart);
    NetSendMessageToClientRpc((byte)ServerMessage.RoundStart);
  }

  [Rpc(SendTo.Everyone)]
  void NetUpdateClientCountsRpc()
  {
    Debug.Log($"<color=orange>There are currently {m_NetWorkManager.ConnectedClients.Count} players in the game</color>");
    SceneData.Singleton.SetTextLobbyPlayerCount(m_NetWorkManager.ConnectedClients.Count);
  }

  [Rpc(SendTo.Server)]
  public void NetSendDataToServerRpc(ulong data, RpcParams rpcParams = default)
  {
    if (!IsServer) return;

    ulong sender = rpcParams.Receive.SenderClientId;
    m_dClientsCardsData[sender] = data;
    m_lClientWaitList.Remove(sender);

#if UNITY_EDITOR
    string answerCards = $"Recieved Following Answer form Client {sender}\n";
    for (int i = 0; i < 5; i++)
    {
      answerCards += $"<color=cyan> {(Symbol)(data & 0x0Ful)} | {(Value)((data & (0x0Ful << 4)) >> 4)}</color>\n";
      data >>= 8;
    }
    Debug.Log(answerCards);
#endif
    return;

    if (m_lClientWaitList.Count <= 0)
    {
      KeyValuePair<ulong, ulong> max = m_dClientsCardsData.OrderByDescending(kvp => kvp.Value).First();

      ulong winner = max.Key;
      List<ulong> loosers = new List<ulong>(m_lConnectedClients);
      loosers.Remove(max.Key);

      NetSendMessageToClientRpc((byte)ServerMessage.ResultDefeat, new ClientRpcParams
      {
        Send = new ClientRpcSendParams { TargetClientIds = loosers.ToArray() }
      });

      NetSendMessageToClientRpc((byte)ServerMessage.ResultVictory, new ClientRpcParams
      {
        Send = new ClientRpcSendParams { TargetClientIds = new[] { max.Key } }
      });
    }
  }

  [ClientRpc(Delivery = RpcDelivery.Reliable, AllowTargetOverride = true)]
  void NetSendMessageToClientRpc(byte msg, ClientRpcParams rpcParams = default)
  {
    if (!IsClient) return;

#if UNITY_EDITOR
    // DebugServerMessage((ServerMessage)msg);
#endif

    switch ((ServerMessage)msg)
    {
      case ServerMessage.GameStart:
        NetworkPlayer.Singleton.OnGameStart();
        return;
      case ServerMessage.RoundEnd:
        break;
      case ServerMessage.RoundStart:
        NetworkPlayer.Singleton.OnRoundStart();
        break;
      case ServerMessage.ResultDefeat:
        return;
      case ServerMessage.ResultVictory:
        return;
      default:
        break;
    }
  }

  public ulong CardsToBytes(ref Card[] cards)
  {
    ulong value = 0;

    for (int i = 0; i < 5; i++)
    {
      value |= ((ulong)((byte)cards[i].symbol | (byte)cards[i].value << 4)) << (i * 8);
    }

    return value;
  }

#if UNITY_EDITOR
  static public void DebugServerMessage(ServerMessage msg)
  {
    Debug.Log($"SERVER MESSAGE | {msg}");
  }
#endif
}
