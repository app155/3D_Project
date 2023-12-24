using Project3D.Lobbies;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project3D.UI
{
    public class UI_MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _mainScreen;
        [SerializeField] private GameObject _joinScreen;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;

        [SerializeField] private Button _submitCodeButton;

        [SerializeField] private TextMeshProUGUI _codeText;
        [SerializeField] private TextMeshProUGUI _NickNameText;

        private void OnEnable()
        {
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _submitCodeButton.onClick.AddListener(OnSubmitCodeClicked);
        }
        private void OnDisable()
        {
            _hostButton.onClick.RemoveListener(OnHostClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
            _submitCodeButton.onClick.RemoveListener(OnSubmitCodeClicked);
        }
        private async void OnHostClicked()
        {
            if (_NickNameText.text != null)
            {
                _mainScreen.SetActive(false);
                bool succeeded = await GameLobbyManager.instance.CreateLobby(_NickNameText.text);

                if (succeeded)
                {
                    // open LobbyUI
                    IUI ui = UIManager.instance.Get<UI_LobbyPanel>();
                    ui.Show();
                }
            }
            else
            {

            }

            

        }

        private void OnJoinClicked()
        {
            _mainScreen.SetActive(false);
            _joinScreen.SetActive(true);
        }

        private async void OnSubmitCodeClicked()
        {

            string code = _codeText.text;
            code = code.Substring(0, code.Length - 1);
            _joinScreen.SetActive(false);
            bool succeeded = await GameLobbyManager.instance.JoinLobby(code);
            if (succeeded)
            {
                // open LobbyUI
                IUI ui = UIManager.instance.Get<UI_LobbyPanel>();
                ui.Show();
            }
        }
    }
}