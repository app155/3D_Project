using Project3D.GameSystem;
using Project3D.Lobbies;
using Project3D.Lobbies.GameFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project3D.UI
{
    public class LobbyUI : UIMonobehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Button _startButton;
        [SerializeField] private Image _mapImage;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _leftButton;
        [SerializeField] private TextMeshProUGUI _MapName;
        [SerializeField] private MapSelectionData _mapSelectionData;


        private int _currentMapIndex = 0;

        private void OnEnable()
        {
            _readyButton.onClick.AddListener(OnReadyPressed);
            if (GameLobbyManager.instance.IsHost)
            {
                _leftButton.onClick.AddListener(OnLeftButtonClicked);
                _rightButton.onClick.AddListener(OnRightButtonClicked);
                _startButton.onClick.AddListener(OnstartButtonClicked);
                Lobbies.GameFramework.LobbyEvent.OnLobbyReady += OnlobbyReady;
            }
            
                
            Lobbies.GameFramework.LobbyEvent.OnLobbyUpdated += OnLobbyUpdated;
        }

        private void OnDisable()
        {
            _readyButton.onClick.RemoveListener(OnReadyPressed);
            _leftButton.onClick.RemoveListener(OnLeftButtonClicked);
            _rightButton.onClick.RemoveListener(OnRightButtonClicked);
            _startButton.onClick.RemoveListener(OnstartButtonClicked);

            Lobbies.GameFramework.LobbyEvent.OnLobbyUpdated -= OnLobbyUpdated;
            Lobbies.GameFramework.LobbyEvent.OnLobbyUpdated -= OnLobbyUpdated;
        }
        private void Start()
        {
            _lobbyCodeText.text = $"Lobby code : {GameLobbyManager.instance.GetLobbyCode()}";    
            _lobbyNameText.text = $"Title : {GameLobbyManager.instance.GetLobbyName()}";

            if (!GameLobbyManager.instance.IsHost)
            {
                _leftButton.gameObject.SetActive(false);
                _rightButton.gameObject.SetActive(false); 
            }
        }

        private async void OnLeftButtonClicked()
        {
            if (_currentMapIndex - 1 < 0)
            {
                _currentMapIndex--;
            }
            else
                _currentMapIndex = 0;

            UpdateMap();
            await GameLobbyManager.instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.maps[_currentMapIndex].sceneName);
        }
        private async void OnRightButtonClicked()    
        {
            if (_currentMapIndex + 1 < _mapSelectionData.maps.Count-1)
            {
                _currentMapIndex++;
            }
            else
                _currentMapIndex = _mapSelectionData.maps.Count-1;

            UpdateMap();
            await GameLobbyManager.instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.maps[_currentMapIndex].sceneName);
        }

        private async void OnReadyPressed()
        {
            bool succeed = await GameLobbyManager.instance.SetPlayerReady();
            if(succeed)
            {
                _readyButton.gameObject.SetActive(false);
            }
        }

        private void UpdateMap()
        {
            _mapImage.color = _mapSelectionData.maps[_currentMapIndex].mapThumnail;
            _MapName.text = _mapSelectionData.maps[_currentMapIndex].mapName;

        }

        private void OnLobbyUpdated()
        {
            _currentMapIndex = GameLobbyManager.instance.GetMapIndex();
            UpdateMap();
        }

        private void OnlobbyReady()
        {
            _startButton.gameObject.SetActive(true);    
        }

        private async void OnstartButtonClicked()
        {
            await GameLobbyManager.instance.StartGame();
        }
        public override void InputAction()
        {
            throw new System.NotImplementedException();
        }
    }
}