using System.Collections.Generic;
using UnityEngine;
using Project3D.Lobbies;
using TMPro;

namespace Project3D.Lobbies
{
    public class LobbySpawner : MonoBehaviour
    {
        [SerializeField] private List<LobbyPlayer> _players;

        private void OnEnable()
        {
          GameFramework.LobbyEvent.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
          GameFramework.LobbyEvent.OnLobbyUpdated -= OnLobbyUpdated;
        }
        private void OnLobbyUpdated()
        {
            List<LobbyPlayerData> playerDatas = GameLobbyManager.instance.Getplayers();

            for(int i=0; i<playerDatas.Count; i++)
            {
                LobbyPlayerData data = playerDatas[i];
                _players[i].SetData(data);
            }
        }
    }
}