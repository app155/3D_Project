using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using TMPro;

namespace Project3D.Lobbies
{
    public class GameLobbyManager : MonoBehaviour
    {
        private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();

        private LobbyPlayerData _localLobbyPlayerData;

        private void OnEnable()
        {
            GameFramework.LobbyEvent.OnLobbyUpdated += OnLobbyUpdated;    
        }


        private void OnDisable()
        {
            GameFramework.LobbyEvent.OnLobbyUpdated -= OnLobbyUpdated;
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
            LobbyPlayerData playerData = new LobbyPlayerData();
            playerData.Initialize(AuthenticationService.Instance.PlayerId, "Host", nickName);
            bool succeeded = await LobbyManager.instance.CreateLobby(roomName, maxPlayer, true, playerData.Serialize());

            return succeeded;
        }


        public async Task<bool> JoinLobby(string code, string nickName)
        {
            LobbyPlayerData playerData = new LobbyPlayerData();
            playerData.Initialize(AuthenticationService.Instance.PlayerId, "Client", nickName);
                
            bool succeeded = await LobbyManager.instance.JoinLobby(code, playerData.Serialize());

            return succeeded;
        }

        public async Task<bool> JoinLobbyById(string _lobbyId, string nickName)
        {
            LobbyPlayerData playerData = new LobbyPlayerData();
            playerData.Initialize(AuthenticationService.Instance.PlayerId, "Client", nickName);

            bool succeeded = await LobbyManager.instance.JoinLobbyById(_lobbyId, playerData.Serialize());

            return succeeded;
        }


        private void OnLobbyUpdated(Lobby lobby)
        {
            List<Dictionary<string, PlayerDataObject>> playerData = LobbyManager.instance.GetPlayerData();
            _lobbyPlayerDatas.Clear();

            foreach(Dictionary<string, PlayerDataObject> data in playerData)
            {
                LobbyPlayerData lobbyPlayerData = new LobbyPlayerData();
                lobbyPlayerData.Initialize(data);

                if(lobbyPlayerData.Id == AuthenticationService.Instance.PlayerId)
                {
                    _localLobbyPlayerData = lobbyPlayerData;
                }

                _lobbyPlayerDatas.Add(lobbyPlayerData);
            }
            //LobbyEvent.OnLobbyUpdated.Invoke();
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
    }
}
