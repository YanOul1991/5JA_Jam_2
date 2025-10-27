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

  private void Start() { }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    Debug.Log("Connected !!!");
  }
}