using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using System.Collections.Specialized;

namespace Project3D.Lobbies
{
    public class GameLobbyManager : MonoBehaviour
    {
        private List<LobbyPlayerData> _lobbyPlayerDatas = new List<LobbyPlayerData>();

        private LobbyPlayerData _localLobbyPlayerData;

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

        public async Task<bool> CreateLobby()
        {
            LobbyPlayerData playerData = new LobbyPlayerData();
            playerData.Initialize(AuthenticationService.Instance.PlayerId, "HostPlayer");
            bool succeeded = await LobbyManager.instance.CreateLobby(4, true, playerData.Serialize());

            return succeeded;
        }


        public async Task<bool> JoinLobby(string code)
        {
            LobbyPlayerData playerData = new LobbyPlayerData();
            playerData.Initialize(AuthenticationService.Instance.PlayerId, "JoinPlayer");
                
            bool succeeded = await LobbyManager.instance.JoinLobby(code, playerData.Serialize());

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

        }
    }
}
