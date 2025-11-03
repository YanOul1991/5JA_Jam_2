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
  ResultDefeat
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

    Debug.Log("NetworkServer Initialized");
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

    if (IsServer)
    {
      NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkClientConnected;
    }
  }

  private void OnNetworkClientConnected(ulong id)
  {
    if (IsServer)
    {
      Debug.Log($"<color=green>New Client connected to server.</color>");

      // Ajoute le nouveau client a la liste
      m_lConnectedClients.Add(id);
      NetUpdateClientCountsRpc();

      // Une fois le nombre de clients atteint on demmare la partie
      if (m_lConnectedClients.Count >= 3)
      {
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
    m_lClientWaitList = new List<ulong>(m_lConnectedClients);

    foreach (ulong client in m_lConnectedClients)
    {
      m_dClientsCardsData.Add(client, 0);
    }

    Debug.Log($"<color=yellow>Client Wait list count {m_lClientWaitList.Count}</color>");

    NetSendMessageToClientRpc((byte)ServerMessage.RoundStart);
  }

  [Rpc(SendTo.Everyone)]
  void NetUpdateClientCountsRpc()
  {
    Debug.Log($"<color=orange>There are currently {m_NetWorkManager.ConnectedClients.Count} players in the game</color>");
    SceneData.Singleton.SetTextLobbyPlayerCount(m_NetWorkManager.ConnectedClients.Count);
  }

  [Rpc(SendTo.Server)]
  void NetSendDataToServerRpc(ulong data, RpcParams rpcParams = default)
  {
    if (!IsServer) return;

    ulong sender = rpcParams.Receive.SenderClientId;
    m_dClientsCardsData[sender] = data;
    m_lClientWaitList.Remove(sender);

    // {(Symbol)(data & 0xF0)}.
    string answerCards = $"Recieved Following Answer form Client {sender}\n";
    for (int i = 0; i < 5; i++)
    {
      answerCards += $"\tCard {i + 1}: {(Value)(data & 0x0Ful)} of {(Symbol)((data & (0x0Ful) << 4) >> 4)}\n";
      data >>= 8;
    }
    // answerCards += $"\tCard 2: {(Value)((data & 0x0Ful << 8) >> 8)} of {(Symbol)((data & (0x0Ful) << 12) >> 12)}\n";
    // answerCards += $"\tCard 3: {(Value)((data & 0x0Ful << 16) >> 16)} of {(Symbol)((data & (0x0Ful) << 20) >> 20)}\n";
    // answerCards += $"\tCard 4: {(Value)((data & 0x0Ful << 24) >> 24)} of {(Symbol)((data & (0x0Ful) << 28) >> 28)}\n";

    Debug.Log(answerCards);

    if (m_lClientWaitList.Count <= 0)
    {
      string message = "";
      message += "Recieved all answers\n";
      message += "Analysing Data:";

      foreach (KeyValuePair<ulong, ulong> pair in m_dClientsCardsData)
      {
        message += $"\nData player {pair.Key} | {pair.Value}";
      }

      Debug.Log(message + '\n');
      KeyValuePair<ulong, ulong> max = m_dClientsCardsData.OrderByDescending(kvp => kvp.Value).First();
      Debug.Log($"Winner is Player {max.Key} with {max.Value}.");

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
  void NetSendMessageToClientRpc(byte serverMessage, ClientRpcParams rpcParams = default)
  {
    if (!IsClient) return;

    ServerMessage msg = (ServerMessage)serverMessage;

    switch (msg)
    {
      case ServerMessage.GameStart:
        Debug.Log($"<color=yellow>Game has started</color>");
        return;

      case ServerMessage.RoundEnd:
        Debug.Log($"<color=orange>Round has ended! A new round will start soon</color>");
        break;

      case ServerMessage.RoundStart:
        Debug.Log($"<color=cyan>Round has Started!!!</color>");
        ulong cardValue = 0;

        for (int i = 0; i < 5; i++)
        {
          cardValue |= (ulong)((byte)(Value)Random.Range(0, (int)Value.Count) | (byte)Symbol.Spades << 4) << (8 * i);
        }
        
        // cardValue |= (ulong)((byte)Value.Ace   | (byte)Symbol.Clubs << 4) << 8;
        // cardValue |= (ulong)((byte)Value.Queen  | (byte)Symbol.Heart    << 4) << 16;
        // cardValue |= (ulong)((byte)Value.Jack    | (byte)Symbol.Diamonds   << 4) << 24;
        
        NetSendDataToServerRpc(cardValue);
        break;

      case ServerMessage.ResultDefeat:
        Debug.Log("<color=red>YOU LOSE!</color>");
        return;

      case ServerMessage.ResultVictory:
        Debug.Log("<color=green>YOU WIN!!!</color>");
        return;

      default:
        Debug.LogError($"Unknown Server Message...");
        break;
    }
  }
}