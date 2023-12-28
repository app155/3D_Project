using Project3D.Lobbies;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project3D.UI
{
    public class UI_MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _mainScreen;
        [SerializeField] private GameObject _createRoomScreen;
        [SerializeField] private GameObject _joinScreen;

        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _roomButton;
        [SerializeField] private Button _submitCodeButton;

        [SerializeField] private TextMeshProUGUI _codeText;
        [SerializeField] private TextMeshProUGUI _nickNameText;
        [SerializeField] private TextMeshProUGUI _roomNameText;
        private void OnEnable()
        {
            _roomButton.onClick.AddListener(OnCreateRoomButtonClicked);
            _hostButton.onClick.AddListener(OnHostClicked);
            _joinButton.onClick.AddListener(OnJoinClicked);
            _submitCodeButton.onClick.AddListener(OnSubmitCodeClicked);
            
        }
        private void OnDisable()
        {
            _roomButton.onClick.RemoveListener(OnCreateRoomButtonClicked);
            _hostButton.onClick.RemoveListener(OnHostClicked);
            _joinButton.onClick.RemoveListener(OnJoinClicked);
            _submitCodeButton.onClick.RemoveListener(OnSubmitCodeClicked);
        }

        private async void OnHostClicked()
        {
            if (_nickNameText.text != null)
            {
                bool succeeded = await GameLobbyManager.instance.CreateLobby(_roomNameText.text, 6, _nickNameText.text);
                _mainScreen.SetActive(false);
                _createRoomScreen.SetActive(false);


                if (succeeded)
                {
                    // open LobbyUI
                    IUI ui = UIManager.instance.Get <LobbyUI>();
                    ui.Show();
                }
            }
            else
            {

            }

            

        }
        private void OnCreateRoomButtonClicked()
        {
            _createRoomScreen.SetActive(true);
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
            bool succeeded = await GameLobbyManager.instance.JoinLobby(code, _nickNameText.text);
            if (succeeded)
            {
                // open LobbyUI
                IUI ui = UIManager.instance.Get<LobbyUI>();
                ui.Show();
            }
        }
    }
}