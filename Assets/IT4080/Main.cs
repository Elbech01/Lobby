using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;

public class Main : NetworkBehaviour
{
    public It4080.NetworkSettings netSettings;
    private Button btnStart;
    


    // Start is called before the first frame update
    void Start(){
        netSettings.startServer += NetSettingsOnServerStart;
        netSettings.startHost += NetSettingsOnHostStart;
        netSettings.startClient += NetSettingsOnClientStart;
        netSettings.setStatusText("Not Connected");
        

        btnStart = GameObject.Find("btnStart").GetComponent<Button>();
        btnStart.onClick.AddListener(btnStartOnClick);
        btnStart.gameObject.SetActive(false);
    }

    
    private void startClient(IPAddress ip, ushort port) {
        var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.ConnectionData.Address = ip.ToString();
        utp.ConnectionData.Port = port;

        NetworkManager.Singleton.OnClientConnectedCallback += ClientOnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientOnClientDisconnected;

        NetworkManager.Singleton.StartClient();
        netSettings.hide();
        Debug.Log("Started client");
        netSettings.setStatusText("Waiting for host");
    }

    
    private void startHost(IPAddress ip, ushort port) {
        var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.ConnectionData.Address = ip.ToString();
        utp.ConnectionData.Port = port;

        NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;

        NetworkManager.Singleton.StartHost();
        netSettings.hide();
        Debug.Log("Started host");
        netSettings.setStatusText($"Server started. We are client {NetworkManager.Singleton.LocalClientId}");

        GoToLobby();
        btnStart.gameObject.SetActive(true);
        
    }

    private void startServer(IPAddress ip, ushort port) {
        var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.ConnectionData.Address = ip.ToString();
        utp.ConnectionData.Port = port;

        NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;

        NetworkManager.Singleton.StartServer();
        netSettings.hide();
        Debug.Log("Started server");
        printIs("startServer");
        netSettings.setStatusText($"Server started. We are client {NetworkManager.Singleton.LocalClientId}");

        GoToLobby();
        btnStart.gameObject.SetActive(true);

    }



    // ------------------------------------
    // Events

    private void btnStartOnClick()
    {
        StartGame();
    }

    private void GoToLobby()
    {
        NetworkManager.SceneManager.LoadScene("Lobby",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void StartGame()
    {
        NetworkManager.SceneManager.LoadScene("Arena1",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void NetSettingsOnServerStart(IPAddress ip, ushort port) {
        startServer(ip, port);
        Debug.Log("Server started");
        printIs("");
    }

    private void NetSettingsOnHostStart(IPAddress ip, ushort port) {
        startHost(ip, port);
        Debug.Log("Host started");
        printIs("");
    }

    private void NetSettingsOnClientStart(IPAddress ip, ushort port) {
        startClient(ip, port);
        Debug.Log("Client started");
        printIs("");
    }

    private void printIs(string msg) {
        Debug.Log($"[{msg}] {MakeIsString()}");
    }
    private string MakeIsString()
    {
        return $"server:{IsServer} host:{IsHost} client:{IsClient} owner:{IsOwner}";
    }

    private void HostOnClientConnected(ulong clientID) {
        Debug.Log($"Client connected to me: {clientID}");
    }

    private void HostOnClientDisconnected(ulong clientID)
    {
        Debug.Log($"Client disconnected from me: {clientID}");
    }

    private void ClientOnClientConnected(ulong clientID) {
        Debug.Log($"Client connected: {clientID}");
    }

    private void ClientOnClientDisconnected(ulong clientID) {
        Debug.Log($"Client disconnected: {clientID}");
    }


}
