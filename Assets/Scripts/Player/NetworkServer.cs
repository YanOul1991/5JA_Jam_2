using UnityEngine;
using Unity.Netcode;

public class NetworkServer : NetworkBehaviour
{
  static public NetworkServer Singleton;

  private void Awake()
  {
    if (Singleton == null)
      Singleton = this;
    else
      Destroy(gameObject);

    Debug.Log("NetworkServer Initialized");
  }
}
