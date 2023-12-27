using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Project3D.Lobbies.Manager;
using UnityEngine.SceneManagement;

namespace Project3D.Lobbies
{
    public class GameLobbyManager : MonoBehaviour
    {
        private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();
        private LobbyPlayerData _localLobbyPlayerData;

        private LobbyData _lobbyData;
        private int _maxNumberOfPlayers;
        public bool IsHost => _localLobbyPlayerData.Id == LobbyManager.instance.GetHostId();

        private void OnEnable()
        {
            LobbyEvent.OnLobbyUpdated += OnLobbyUpdated;    
        }


        private void OnDisable()
        {
            LobbyEvent.OnLobbyUpdated -= OnLobbyUpdated;
        }

        public string GetLobbyCode()
        {
            return LobbyManager.instance.GetLobbyCode();
        }
        public string GetLobbyName()
        {
            return LobbyManager.instance.GetLobbyName();
        }
        public static GameLobbyManager instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("LobbyManager");
                    _instance = go.AddComponent<GameLobbyManager>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private static GameLobbyManager _instance;

        public async Task<bool> CreateLobby(string roomName, int maxPlayer, string nickName)
        {
            _localLobbyPlayerData = new LobbyPlayerData();
            _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "Host", nickName);

            _lobbyData = new LobbyData();
            _lobbyData.Initialize(0);
            _maxNumberOfPlayers = maxPlayer;
            bool succeeded = await LobbyManager.instance.CreateLobby(roomName, _maxNumberOfPlayers, true, _localLobbyPlayerData.Serialize(), _lobbyData.Serialize());

            return succeeded;
        }


        public async Task<bool> JoinLobby(string code, string nickName)
        {
            _localLobbyPlayerData = new LobbyPlayerData();
            _localLobbyPlayerData.Initialize(AuthenticationService.Instance.PlayerId, "Client", nickName);
                
            bool succeeded = await LobbyManager.instance.JoinLobby(code, _localLobbyPlayerData.Serialize());

            return succeeded;
        }

        public async Task<bool> JoinLobbyById(string _lobbyId, string nickName)
        {
            LobbyPlayerData playerData = new LobbyPlayerData();
            playerData.Initialize(AuthenticationService.Instance.PlayerId, "Client", nickName);

            bool succeeded = await LobbyManager.instance.JoinLobbyById(_lobbyId, playerData.Serialize());

            return succeeded;
        }


        private async void OnLobbyUpdated(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.instance.GetPlayerData();
            _lobbyPlayerDatas.Clear();

            int numberOfPlayerReady = 0;

            foreach(Dictionary<string, PlayerDataObject> data in playerData)
            {
                LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
                lobbyPlayerData.Initialize(data);

                if (lobbyPlayerData.IsReady)
                {
                    numberOfPlayerReady++;
                }
                if(lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
                {
                    _localLobbyPlayerData = lobbyPlayerData;
                }

                _lobbyPlayerDatas.Add(lobbyPlayerData);
            }

            _lobbyData = new LobbyData();
            _lobbyData.Initialize(lobby.Data);

            GameFramework.LobbyEvent.OnLobbyUpdated?.Invoke();

            if(numberOfPlayerReady == lobby.Players.Count)
            {
                GameFramework.LobbyEvent.OnLobbyReady?.Invoke();
            }

            if(_lobbyData.RelayJoinCode != default) 
            {
                await JoinRelayServer(_lobbyData.RelayJoinCode);
                SceneManager.LoadSceneAsync(_lobbyData.SceneName);
            }
        }

        public List<LobbyPlayerData> Getplayers()
        {
            return _lobbyPlayerDatas;
        }
        public async Task<bool> SetPlayerReady()
        {
            _localLobbyPlayerData.IsReady = true;
            return await LobbyManager.instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
        }

        public async Task<bool> SetPlayerTeam(int team)
        {
            _localLobbyPlayerData.Team = team;
            return await LobbyManager.instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize());
        }

        public int GetMapIndex()
        {
            return _lobbyData.MapIndex;
        }

        public async Task<bool> SetSelectedMap(int currentMapIndex, string sceneName)
        {
            _lobbyData.MapIndex = currentMapIndex;
            _lobbyData.SceneName = sceneName;
            return await LobbyManager.instance.UpdateLobbyData(_lobbyData.Serialize());

        }

        public async Task StartGame()
        {
            string relayJoinCode = await RelayManager.Instance.CreateRelay(_maxNumberOfPlayers);

            _lobbyData.RelayJoinCode = relayJoinCode;
            await LobbyManager.instance.UpdateLobbyData(_lobbyData.Serialize());

            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();
            await LobbyManager.instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);

            SceneManager.LoadSceneAsync(_lobbyData.SceneName);
        }

        private async Task<bool> JoinRelayServer(string relayJoinCode)
        {
            await RelayManager.Instance.JoinRelay(relayJoinCode);
            string allocationId = RelayManager.Instance.GetAllocationId();
            string connectionData = RelayManager.Instance.GetConnectionData();
            await LobbyManager.instance.UpdatePlayerData(_localLobbyPlayerData.Id, _localLobbyPlayerData.Serialize(), allocationId, connectionData);
            return true;
        }
    }
}
