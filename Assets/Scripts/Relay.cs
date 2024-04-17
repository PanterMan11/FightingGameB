using System;
using System.Drawing.Text;
using System.Net.Security;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class Relay : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_InputField joinInput;
    [SerializeField] private GameObject buttons;
    private UnityTransport transport;
    private const int MaxPlayers = 2;


    private async void Awake()
    {
        transport = FindObjectOfType<UnityTransport>();
        buttons.SetActive(false);
        await Authenticate();
        buttons.SetActive(true);
    }

    private async Task Authenticate()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateGame()
    {
        buttons.SetActive(false);
        Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
        joinCodeText.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame()
    {
        buttons.SetActive(false);

        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCodeText.text);

        transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
        NetworkManager.Singleton.StartClient();
    }

}


