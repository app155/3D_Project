using Project3D.GameSystem;
using Project3D.Lobbies;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Project3D.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        private void Start()
        {
            _lobbyCodeText.text = $"Lobby code : {GameLobbyManager.instance.GetLobbyCode()}";    
            _lobbyNameText.text = $"Title : {GameLobbyManager.instance.GetLobbyName()}";
        }
    }
}