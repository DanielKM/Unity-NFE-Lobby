using System;
using QFSW.QC;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    private Lobby hostLobby;
    private float heartbeatTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) // 15 seconds
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;
                
                Debug.Log($"Sending heartbeat ping to lobby {hostLobby.Name} with ID: {hostLobby.Id}");
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    [Command]
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            hostLobby = lobby;
            Debug.Log($"Lobby created: {lobby.Name} with ID: {lobby.Id}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
            return;
        }
    }

    [Command]
    private async void ListLobbies()
    {
        try {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            Debug.Log($"Found {queryResponse.Results.Count} lobbies:");

            foreach (var lobby in queryResponse.Results)
            {
                Debug.Log($"Lobby Name: {lobby.Name}, ID: {lobby.Id}, Players: {lobby.Players.Count}/{lobby.MaxPlayers}");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
            return;
        }
    }
}
