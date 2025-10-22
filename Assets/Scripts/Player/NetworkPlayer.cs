using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
  static public NetworkPlayer Singleton;

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
  
  private void Start()
  {
    NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkClientConnected;
  }

  private void OnNetworkClientConnected(ulong id)
  {
    Debug.Log($"A client has connected | current connected clients count : {NetworkManager.Singleton.ConnectedClients.Count}");
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    Debug.Log("Connected !!!");
  }
}