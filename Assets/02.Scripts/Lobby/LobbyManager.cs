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

        public string GetLobbyCode()
        {
            return _lobby?.LobbyCode;
            
        }
        public string GetLobbyName()
        {
           
            return _lobby.Name;
        }
        public async Task<bool> CreateLobby(string roomName, int maxPlayers, bool isPrivate, Dictionary<string, string> data, Dictionary<string, string> lobbyData)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

            Player player = new Player(AuthenticationService.Instance.PlayerId, connectionInfo: null, playerData);

            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                Data = SerializeLobbyData(lobbyData),
                IsPrivate = isPrivate,
                Player = player,
            };

            try
            {
                _lobby = await LobbyService.Instance.CreateLobbyAsync(roomName, maxPlayers, options);
                
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
                    LobbyEvent.OnLobbyUpdated?.Invoke(_lobby);
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

        private Dictionary<string, DataObject> SerializeLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
            foreach(var(key, value) in data)
            {
                lobbyData.Add(key, new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value : value));
            }
            return lobbyData;
        }
        private void OnApplicationQuit()
        {
            if (_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
            }
        }

        internal async Task<bool> JoinLobby(string code, Dictionary<string, string> playerData)
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData));

            options.Player = player;

            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);

            }
            catch (Exception)
            {
                return false;
            }

            _refreshLobbyCoroutine = StartCoroutine(C_RefreshLobby(_lobby.Id, 1.0f));
            return true;
        }

        internal async Task<bool> JoinLobbyById(string _lobbyId, Dictionary<string, string> playerData)
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
            Player player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData));

            options.Player = player;

            try
            {
                _lobby = await LobbyService.Instance.JoinLobbyByIdAsync(_lobbyId, options);

            }
            catch (Exception)
            {
                return false;
            }

            _refreshLobbyCoroutine = StartCoroutine(C_RefreshLobby(_lobby.Id, 1.0f));
            return true;
        }

        public List<Dictionary<string, PlayerDataObject>> GetPlayerData()
        {
            List<Dictionary<string, PlayerDataObject>> data = new List<Dictionary<string, PlayerDataObject>>();

            foreach(Player player in _lobby.Players)
            {
                data.Add(player.Data);

            }
            return data;
        }

        public async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data, string allocationId = default, string connectionData = default)
        {
            Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
            UpdatePlayerOptions options = new UpdatePlayerOptions()
            {
                Data = playerData,
                AllocationId = allocationId,
                ConnectionInfo = connectionData
            };

            try
            {
                await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);
            }
            catch (System.Exception)
            {
                return false;
            }

            LobbyEvent.OnLobbyUpdated(_lobby);
            return true;    
        }

        public async Task<bool> UpdateLobbyData(Dictionary<string, string> data)
        {
            Dictionary<string ,DataObject> lobbyData = SerializeLobbyData(data);

            UpdateLobbyOptions options = new UpdateLobbyOptions()
            {
                Data = lobbyData
            };

            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(_lobby.Id, options);
            }
            catch (Exception)
            {
                return false;
            }

            LobbyEvent.OnLobbyUpdated(_lobby);
            return true;

        }

        public string GetHostId()
        {
            return _lobby.HostId;
        }
    }
}