using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public enum ServerMessage : byte
{
  GameStart,
  RoundEnd,
  RoundStart
}

public class NetworkServer : NetworkBehaviour
{
  static public NetworkServer Singleton;
  private NetworkManager m_NetWorkManager;

  private List<ulong> ConnectedClients;

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
    ConnectedClients = new List<ulong>();
  }

  private void Start()
  {
    m_NetWorkManager = NetworkManager.Singleton;
    NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkClientConnected;
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    SceneData.Singleton.TextLobbyPlayerCount.gameObject.SetActive(true);
  }

  private void OnNetworkClientConnected(ulong id)
  {
    if (IsServer)
    {
      ConnectedClients.Add(id);
      NetUpdateClientCountsRpc();
      if (ConnectedClients.Count >= 3)
      {
        NetSendMessageToClientRpc((byte)ServerMessage.GameStart);
        RoundStart();
      }
    }
  }

  private void RoundStart()
  {
    if (!IsServer) return;
    // ClientRpcParams rpcParams = new ClientRpcParams
    // {
    //   Send = new ClientRpcSendParams
    //   {
    //     TargetClientIds = new[] { ConnectedClients[1] },
    //   }
    // };
    m_dClientsCardsData = new Dictionary<ulong, ulong>();
    m_lClientWaitList = new List<ulong>();

    foreach (ulong client in ConnectedClients)
    {
      m_dClientsCardsData.Add(client, 0);
      m_lClientWaitList.Add(client);
    }

    // NetSendPrivateMessageClientRpc(rpcParams);
    NetSendMessageToClientRpc((byte)ServerMessage.RoundStart);
  }

  [Rpc(SendTo.Everyone)]
  void NetUpdateClientCountsRpc()
  {
    Debug.Log($"There are currently {m_NetWorkManager.ConnectedClients.Count} players in the game");
    SceneData.Singleton.SetTextLobbyPlayerCount(m_NetWorkManager.ConnectedClients.Count);
  }

  [Rpc(SendTo.Server)]
  void NetSendDataToServerRpc(ulong data, RpcParams rpcParams = default)
  {
    if (!IsServer) return;

    Debug.Log("Answer Recieved.");
    ulong sender = rpcParams.Receive.SenderClientId;

    m_dClientsCardsData[sender] = data;
    m_lClientWaitList.Remove(sender);

    string message = "";
    if (m_lClientWaitList.Count <= 0)
    {
      message += "Recieved all answers\n";
      message += "\tAnalysing Data:\n";

      foreach (KeyValuePair<ulong, ulong> pair in m_dClientsCardsData)
      {
        message += $"\t\t\tData player {pair.Key} | {pair.Value}\n";
      }

      Debug.Log(message);
      Invoke(nameof(RoundStart), 5.0f);
    }
    else
    {
      Debug.Log("Waiting for all clients answers...");
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
        Debug.Log($"<color=cyan>Game has started</color>");
        break;
      case ServerMessage.RoundEnd:
        Debug.Log($"<color=cyan>Round has ended!</color>");
        break;
      case ServerMessage.RoundStart:
        Debug.Log($"<color=cyan>Round has Started!!!</color>");
        break;
      default:
        Debug.LogError($"Unknown Server Message...");
        break;
    }
    
    NetSendDataToServerRpc((ulong)Random.Range(0, 200));
  }
}