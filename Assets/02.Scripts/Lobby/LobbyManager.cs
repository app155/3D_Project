using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Project3D.Lobbies
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("LobbyManager");
                    _instance = go.AddComponent<LobbyManager>();
                    DontDestroyOnLoad(go);
                }
                
                return _instance;
            }
        }

        private static LobbyManager _instance;
        private Lobby _lobby;
        private Coroutine _heartbeatCoroutine;
        private Coroutine _refreshLobbyCoroutine;

        public async Task<bool> CreateLobby(int maxPlayers, bool isPrivate, Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

            Player player = new Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                IsPrivate = isPrivate,
                Player = player,
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync("Lobby", maxPlayers, options);
            }

            catch (Exception)
            {
                return false;
            }

            Debug.Log($"lobby created with lobby id {_lobby.Id}");

            _heartbeatCoroutine = StartCoroutine(C_HeartBeatLobby(_lobby.Id, 6.0f));
            _refreshLobbyCoroutine = StartCoroutine(C_RefreshLobby(_lobby.Id, 1.0f));

            return true;
        }

        private IEnumerator C_HeartBeatLobby(string lobbyID, float waitSeconds)
        {
            while (true)
            {
                Debug.Log("Heartbeat");
                LobbyService.Instance.SendHeartbeatPingAsync(lobbyID);

                yield return new WaitForSecondsRealtime(waitSeconds);
            }
        }

        private IEnumerator C_RefreshLobby(string lobbyID, float waitSeconds)
        {
            while (true)
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyID);

                yield return new WaitUntil(() => task.IsCompleted);

                Lobby newLobby = task.Result;

                if (newLobby.LastUpdated > _lobby.LastUpdated)
                {
                    _lobby = newLobby;
                }

                yield return new WaitForSecondsRealtime(waitSeconds);
            }
        }

        private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
        {
            Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();

            foreach(var (key, value) in data)
            {
                playerData.Add(key, new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member,
                                                         value: value));
            }

            return playerData;
        }

        private void OnApplicationQuit()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
        }
    }
}