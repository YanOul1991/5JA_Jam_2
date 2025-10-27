using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;
using TMPro;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
  public static RelayManager Singleton;
  const int m_MaxConnections = 4;
  public string RelayJoinCode;
  private Allocation allocation;
  private JoinAllocation joinAllocation;

  [SerializeField] private TextMeshProUGUI joinCodeText;
  [SerializeField] private TMP_InputField joinCodeInputField;
  
  [SerializeField] private Button btnStartHost;
  [SerializeField] private Button btnStartClient;

  void Awake()
  {
    if (Singleton == null) Singleton = this;
    else Destroy(gameObject);
  }

  void Start()
  {
    AuthenticatePlayer();
    btnStartHost.onClick.AddListener(OnStartHostClick);
    btnStartClient.onClick.AddListener(OnStartClientClick);
    joinCodeInputField.onEndEdit.AddListener(ConfigureClientStart);
  }

  private void OnStartHostClick()
  {
    Debug.Log($"Starting host...");
    btnStartHost.onClick.RemoveListener(OnStartHostClick);
    btnStartClient.onClick.RemoveListener(OnStartClientClick);
    StartCoroutine(ConfigureTransportAndStartNgoAsHost());
  }

  private void OnStartClientClick()
  {
    Debug.Log($"Starting as client...");
    btnStartHost.onClick.RemoveListener(OnStartHostClick);
    btnStartClient.onClick.RemoveListener(OnStartClientClick);
    joinCodeInputField.onEndEdit.AddListener(ConfigureClientStart);
  }

  private void ConfigureClientStart(string value)
  {
    RelayJoinCode = value;
    joinCodeInputField.onEndEdit.RemoveListener(ConfigureClientStart);
    StartCoroutine(ConfigureTransportAndStartNgoAsConnectingPlayer());
  }

  async void AuthenticatePlayer()
  {
    try
    {
      await UnityServices.InitializeAsync();
      await AuthenticationService.Instance.SignInAnonymouslyAsync();
      var playerID = AuthenticationService.Instance.PlayerId;
    }
    catch (Exception e)
    {
      Debug.LogException(e);
    }
  }

  // ///////////////////////////////// HOST SERVER CONNECTION
  public async Task<string> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
  {
    string createJoinCode;
    try
    {
      allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
    }
    catch (Exception e)
    {
      Debug.LogError($"Relay create allocation request failed {e.Message}");
      throw;
    }

    Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
    Debug.Log($"server: {allocation.AllocationId}");

    try
    {
      createJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
      joinCodeText.text = createJoinCode;
    }
    catch
    {
      Debug.LogError("Relay create join code request failed");
      throw;
    }
    return createJoinCode;
  }

  public IEnumerator ConfigureTransportAndStartNgoAsHost()
  {
    var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(m_MaxConnections);
    while (!serverRelayUtilityTask.IsCompleted)
    {
      yield return null;
    }

    if (serverRelayUtilityTask.IsFaulted)
    {
      Debug.LogError("Exception thrown when attempting to start relay server. Server not started. Exception: " + serverRelayUtilityTask.Exception.Message);
      yield break;
    }

    var relayServerData = serverRelayUtilityTask.Result;

    
    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
    NetworkManager.Singleton.StartHost();
    yield return null;
  }
  
  // ///////////////////////////////// CLIENT SERVER CONNECTION
  public async Task<JoinAllocation> JoinRelayServerFromJoinCode(string joincode)
  {

    try
    {
      joinAllocation = await RelayService.Instance.JoinAllocationAsync(joincode);
    }
    catch
    {
      Debug.LogError($"Relay create join code request failed");
      throw;
    }

    Debug.Log($"client: {joinAllocation.ConnectionData[0]} {joinAllocation.ConnectionData[1]}");
    Debug.Log($"host: {joinAllocation.HostConnectionData[0]} {joinAllocation.HostConnectionData[1]}");
    Debug.Log($"client: {joinAllocation.AllocationId}");

    return joinAllocation;
    //return new RelayServerData(allocation, "dtls");
  }

  public IEnumerator ConfigureTransportAndStartNgoAsConnectingPlayer()
  {
    // Start a JoinRelayServerFromJoinCode Task from the given join code in the input field
    Task<JoinAllocation> clientRelayUtilityTask = JoinRelayServerFromJoinCode(joinCodeInputField.text);

    // Wait for task to complete
    while (!clientRelayUtilityTask.IsCompleted) yield return null;

    // Display Error
    if (clientRelayUtilityTask.IsFaulted)
    {
      Debug.LogError("Exception thrown when attempting to connect to Relay Server. Exception: " + clientRelayUtilityTask.Exception.Message);
      yield break;
    }
    
    // var relayServerData = clientRelayUtilityTask.Result;
    // Join sever and connect as client
    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));
    NetworkManager.Singleton.StartClient();
    yield return null;
  }
}