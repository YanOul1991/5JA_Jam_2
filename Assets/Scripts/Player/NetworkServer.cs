using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkServer : NetworkBehaviour
{
  static public NetworkServer Singleton;

  private List<ulong> ConnectedClients;

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
    NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkClientConnected;
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
  }

  private void OnNetworkClientConnected(ulong id)
  {
    if (IsServer)
    {
      ConnectedClients.Add(id);
      NetUpdateClientCountsRpc(ConnectedClients.Count);
    }
  }

  [Rpc(SendTo.Everyone)]
  void NetUpdateClientCountsRpc(int count)
  {
    Debug.Log($"There are currently {count} players in the game");
  }
}
