using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;
using Project3D.Lobbies;

namespace Project3D.UI
{
    public class UI_LobbyPanel : UIMonobehaviour
    {

        [SerializeField] private GameObject lobbyInfoPrefab;
        [SerializeField] private GameObject lobbiesInfoContent;
        [SerializeField] private GameObject lobbyListScreen;

        [SerializeField] private TextMeshProUGUI nickName;
        [SerializeField] private Button GetLobbiesList;
        [SerializeField] private Button _joinWithCode;
        [SerializeField] private GameObject _joinScreen;

        private float updateLobbiesListTimer = 2f;


        private void OnEnable()
        {
            GetLobbiesList.onClick.AddListener(ListPublicLobbies);
            _joinWithCode.onClick.AddListener(OnJoinClicked);
        }
        private void OnDisable()
        {
            GetLobbiesList.onClick.RemoveListener(ListPublicLobbies);
            _joinWithCode.onClick.RemoveListener(OnJoinClicked);

        }
        private void Update()
        {
            HandleLobbiesListUpdate();
        }
        private async void ListPublicLobbies()
        {
            try
            {
                QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
                VisualizeLobbyList(response.Results);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        private void HandleLobbiesListUpdate()
        {
            updateLobbiesListTimer -= Time.deltaTime;
            if (updateLobbiesListTimer <= 0)
            {
                ListPublicLobbies();
                updateLobbiesListTimer = 2f;
            }
        }
        private void VisualizeLobbyList(List<Lobby> _publicLobbies)
        {
            // We need to clear previous info
            for (int i = 0; i < lobbiesInfoContent.transform.childCount; i++)
            {
                Destroy(lobbiesInfoContent.transform.GetChild(i).gameObject);
            }
            foreach (Lobby _lobby in _publicLobbies)
            {
                GameObject newLobbyInfo = Instantiate(lobbyInfoPrefab, lobbiesInfoContent.transform);
                var lobbyDetailsTexts = newLobbyInfo.GetComponentsInChildren<TextMeshProUGUI>();
                lobbyDetailsTexts[0].text = _lobby.Name;
                lobbyDetailsTexts[1].text = (_lobby.MaxPlayers - _lobby.AvailableSlots).ToString() + "/" + _lobby.MaxPlayers.ToString();
                newLobbyInfo.GetComponentInChildren<Button>().onClick.AddListener(() => LobbyClicked(_lobby));
            }
        }
        private void OnJoinClicked()
        {
            _joinScreen.SetActive(true);
        }

        private async void LobbyClicked(Lobby lobby)
        {
            bool successed = await GameLobbyManager.instance.JoinLobbyById(lobby.Id, nickName.text);
            Debug.Log(successed);
            if(successed)
            {

                // open LobbyUI
                IUI ui = UIManager.instance.Get<LobbyUI>();
                ui.Show();
            }
            lobbyListScreen.SetActive(false);

        }
        public override void InputAction()
        {
            
        }
    }
}