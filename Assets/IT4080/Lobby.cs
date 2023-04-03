using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Lobby : NetworkBehaviour
{
    public It4080.ConnectedPlayers connectedPlayers;
    private Button btnStart;

    private void Start()
    {
        btnStart = GameObject.Find("start").GetComponent<Button>();
        btnStart.onClick.AddListener(btnStartOnClick);
        btnStart.gameObject.SetActive(IsHost);
    }
    public override void OnNetworkSpawn()
    {
        InitialClear();
        if (IsClient)
        {
            NetworkHandler.Singleton.allPlayers.OnListChanged += ClientOnAllPlayersChanged;
            AddPlayersUsingPlayerData(NetworkHandler.Singleton.allPlayers);

            if (IsHost)
            {
                NetworkManager.OnClientDisconnectCallback += ClientOnDisconnect;
            }
        }
       

    }

    public void InitialClear()
    {
        connectedPlayers.Clear();
    }

    public void AddPlayersUsingPlayerData(NetworkList<It4080.PlayerData> players)
    {
        connectedPlayers.Clear();
        
        foreach (It4080.PlayerData p in players)
        {
            var card = AddPlayerCard(p.clientId);
            card.SetReady(p.isReady);
            string status = "Not Ready";
            if (p.isReady)
            {
                status = "Ready";
            }
            card.SetStatus(status);
           
        }
    }

    private It4080.PlayerCard AddPlayerCard(ulong clientId)
    {
        It4080.PlayerCard card = connectedPlayers.AddPlayer("temp", clientId);
        string you = "";
        string what = "";
        card.ShowKick(IsServer);

        if (clientId == NetworkManager.LocalClientId)
        {
            you = "(you)";
            card.ShowReady(true);
            //card.ReadyToggled += ClientOnReadyToggled;
        }
        else
        {
            you = "";
            card.ShowReady(false);
        }

        if (clientId == NetworkManager.ServerClientId)
        {
            what = "Host";
            card.ShowReady(false);
            card.ShowKick(false);
        }
        else
        {
            what = "Player";
        }
        card.SetPlayerName($"{what} {clientId} {you}");
        return card;
    }

    private void ClientOnDisconnect(ulong clientId)
    {
        
    }

    private void ClientOnAllPlayersChanged(NetworkListEvent<It4080.PlayerData> change)
    {
        AddPlayersUsingPlayerData(NetworkHandler.Singleton.allPlayers);
    }

    private void ClientOnReadyToggled(ulong clientId)
    {
        
    }

    private void btnStartOnClick()
    {
        StartGame();
    }

    private void StartGame()
    {
        NetworkManager.SceneManager.LoadScene("Arena1",
            UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void EnableStartIfAllReady()
    {
        int ready = 0;
        foreach (It4080.PlayerData players in NetworkHandler.Singleton.allPlayers)
        {
            if (players.isReady)
            {
                ready++;
            }
        }

        btnStart.enabled = ready == NetworkHandler.Singleton.allPlayers.Count;
        if (btnStart.enabled)
        {
            btnStart.GetComponentInChildren<TextMeshProUGUI>().text = "Start Game";
        } else
        {
            btnStart.GetComponentInChildren<TextMeshProUGUI>().text = "Players Not Ready";
        }
    }
}
