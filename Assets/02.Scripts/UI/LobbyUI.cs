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
        [SerializeField] private Button _setRedTeam;
        [SerializeField] private Button _setBlueTeam;
        [SerializeField] private TextMeshProUGUI _MapName;
        [SerializeField] private MapSelectionData _mapSelectionData;

        [SerializeField] private Button _characterChoice;
        [SerializeField] private Button _characterOne;
        [SerializeField] private Button _characterTwo;
        [SerializeField] private Button _characterSubmit;
        [SerializeField] private GameObject _characterChoiceScreen;
        [SerializeField] private GameObject _characterOneScreen;
        [SerializeField] private GameObject _characterTwoScreen;

        private int _currentMapIndex = 0;

        private int _setTeamBlue = 0;
        private int _setTeamRed = 0;

        private void OnEnable()
        {
            _readyButton.onClick.AddListener(OnReadyPressed);
            _setRedTeam.onClick.AddListener(OnSetRedButton);
            _setBlueTeam.onClick.AddListener(OnSetBlueButton);
            _characterChoice.onClick.AddListener(OnCharacterChoice);
            _characterOne.onClick.AddListener(OnCharacterOne);
            _characterTwo.onClick.AddListener(OnCharacterTwo);
            _characterSubmit.onClick.AddListener(OnCharacterSubmit);

            if (GameLobbyManager.instance.IsHost)
            {
                _leftButton.onClick.AddListener(OnLeftButtonClicked);
                _rightButton.onClick.AddListener(OnRightButtonClicked);
                _startButton.onClick.AddListener(OnstartButtonClicked);

                Lobbies.GameFramework.LobbyEvent.OnLobbyReady += OnLobbyReady;
            }

            Lobbies.GameFramework.LobbyEvent.OnLobbyUpdated += OnLobbyUpdated;

        }

        private void OnDisable()
        {
            _readyButton.onClick.RemoveListener(OnReadyPressed);
            _leftButton.onClick.RemoveListener(OnLeftButtonClicked);
            _rightButton.onClick.RemoveListener(OnRightButtonClicked);
            _startButton.onClick.RemoveListener(OnstartButtonClicked);
            _setRedTeam.onClick.RemoveListener(OnSetRedButton);
            _setBlueTeam.onClick.RemoveListener(OnSetBlueButton);
            _characterChoice.onClick.RemoveListener(OnCharacterChoice);
            _characterOne.onClick.RemoveListener(OnCharacterOne);
            _characterTwo.onClick.RemoveListener(OnCharacterTwo);
            _characterSubmit.onClick.RemoveListener(OnCharacterSubmit);

            Lobbies.GameFramework.LobbyEvent.OnLobbyUpdated -= OnLobbyUpdated;
            Lobbies.GameFramework.LobbyEvent.OnLobbyReady -= OnLobbyReady;
        }
        private async void Start()
        {
            _lobbyCodeText.text = $"Lobby code : {GameLobbyManager.instance.GetLobbyCode()}";    
            _lobbyNameText.text = $"Title : {GameLobbyManager.instance.GetLobbyName()}";

            if (!GameLobbyManager.instance.IsHost)
            {
                _leftButton.gameObject.SetActive(false);
                _rightButton.gameObject.SetActive(false); 
            }
            else
            {
                 await GameLobbyManager.instance.SetSelectedMap(_currentMapIndex, _mapSelectionData.maps[_currentMapIndex].sceneName);
            }

            if (_setTeamBlue >= _setTeamRed)
            {
                await GameLobbyManager.instance.SetPlayerTeam(0);
                _setTeamRed++;
            }
            else
            {
                await GameLobbyManager.instance.SetPlayerTeam(1);
                _setTeamBlue++;
            }
        }

        private void OnCharacterChoice()
        {
            _characterChoiceScreen.SetActive(true);
        }

        private async void OnCharacterOne()
        {
            await GameLobbyManager.instance.SetPlayerCharacter(0);

            Debug.Log(GameLobbyManager.instance.LocalLobbyPlayerData.Character);
            _characterOneScreen.SetActive(true);
            _characterTwoScreen.SetActive(false);
        }

        private async void OnCharacterTwo()
        {
            await GameLobbyManager.instance.SetPlayerCharacter(1);
            _characterOneScreen.SetActive(false);
            _characterTwoScreen.SetActive(true);
        }

        private void OnCharacterSubmit()
        {
            _characterChoiceScreen.SetActive(false);

        }
        private async void OnSetRedButton()
        {
            bool success = await GameLobbyManager.instance.SetPlayerTeam(0);
            
            if (success)
            {
                _setTeamRed++;
                _setTeamBlue--;
            }

        }
        private async void OnSetBlueButton()
        {
            bool success = await GameLobbyManager.instance.SetPlayerTeam(1);
            if (success)
            {
                _setTeamRed--;
                _setTeamBlue++;
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

        private void OnLobbyReady()
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