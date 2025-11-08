using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public enum ServerMessage : byte
{
  GameStart,
  RoundStart,
  ResultVictory,
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

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();

    SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
    SceneData.Singleton.textGameStatus.text = $"En attente de joueurs {NetworkManager.Singleton.ConnectedClientsList.Count}/4";
    if (IsServer) NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkClientConnected;

  }

  public override void OnNetworkDespawn()
  {
    base.OnNetworkDespawn();
  }

  private void OnNetworkClientConnected(ulong id)
  {
    SceneData.Singleton.textGameStatus.text = $"En attente de joueurs {NetworkManager.Singleton.ConnectedClientsList.Count}/4";
    if (IsServer)
    {
      m_lConnectedClients.Add(id);

      if (m_lConnectedClients.Count >= 2)
      {
        SceneData.Singleton.textGameStatus.gameObject.SetActive(false);
        NetworkManager.Singleton.OnClientConnectedCallback -= OnNetworkClientConnected;
        NetSendMessageToClientRpc((byte)ServerMessage.GameStart);
        RoundStart();
      }
    }
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

// #if UNITY_EDITOR
//     string _strDebug = "<color=cyan>The Following cards have been randomly picked</color>\n";
//     foreach (byte cardByte in m_lPickedCards)
//     {
//       _strDebug += $"{(Value)(cardByte & 0x0F)} | {(Symbol)((cardByte & (0x0Fu << 4)) >> 4)}\n";
//     }
//     Debug.Log(_strDebug);
// #endif

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
      // KeyValuePair<ulong, int> max = m_dClientsPoints.OrderByDescending(kvp => kvp.Value).First();

      ulong winner = m_dClientsPoints.OrderByDescending(kvp => kvp.Value).First().Key;
      List<ulong> loosers = new List<ulong>(m_lConnectedClients);
      loosers.Remove(winner);

      NetSendMessageToClientRpc((byte)ServerMessage.ResultDefeat, new ClientRpcParams
      {
        Send = new ClientRpcSendParams { TargetClientIds = loosers.ToArray() }
      });

      NetSendMessageToClientRpc((byte)ServerMessage.ResultVictory, new ClientRpcParams
      {
        Send = new ClientRpcSendParams { TargetClientIds = new[] { winner } }
      });
#if UNITY_EDITOR
      Debug.Log("A new round will start soon...");
      Invoke(nameof(RoundStart), 10.0f);
#endif
    }
  }

  [ClientRpc(Delivery = RpcDelivery.Reliable, AllowTargetOverride = true)]
  void NetSendMessageToClientRpc(byte msg, ClientRpcParams rpcParams = default)
  {
    if (!IsClient) return;

#if UNITY_EDITOR
    DebugServerMessage((ServerMessage)msg);
#endif
    switch ((ServerMessage)msg)
    {
      case ServerMessage.GameStart:
        NetworkPlayer.Singleton.OnGameStart();
        return;
      case ServerMessage.RoundStart:
        SceneData.Singleton.textGameStatus.gameObject.SetActive(false);
        NetworkPlayer.Singleton.OnRoundStart();
        break;
      case ServerMessage.ResultDefeat:
        SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
        SceneData.Singleton.textGameStatus.text = "Round perdu";
        return;
      case ServerMessage.ResultVictory:
        SceneData.Singleton.textGameStatus.gameObject.SetActive(true);
        SceneData.Singleton.textGameStatus.text = "Round Gagne!";
        return;
      case ServerMessage.Wait:
        NetworkPlayer.Singleton.OnRoundEnd();
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
      value |= ((ulong)((byte)cards[i].value | (byte)cards[i].symbol << 4)) << (i * 8);
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
