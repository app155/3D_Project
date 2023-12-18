using Project3D.Lobbies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project3D.UI
{
    public class UI_MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;

        private void Start()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
        }

        private async void OnHostClicked()
        {
            bool succeeded = await GameLobbyManager.instance.CreateLobby();

            if (succeeded)
            {
                // open LobbyUI
                IUI ui = UIManager.instance.Get<UI_LobbyPanel>();
                ui.Show();
            }
        }

        private void OnJoinClicked()
        {

        }
    }
}