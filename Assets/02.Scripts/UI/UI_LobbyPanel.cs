using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;
using Project3D.Lobbies;
using TMPro;
namespace Project3D.UI
{
    public class UI_LobbyPanel : UIMonobehaviour
    {

        [SerializeField] private GameObject lobbyInfoPrefab;
        [SerializeField] private GameObject lobbiesInfoContent;
        [SerializeField] private TextMeshProUGUI nickName;
        [SerializeField] private Button GetLobbiesList;

        private void OnEnable()
        {
            GetLobbiesList.onClick.AddListener(ListPublicLobbies);

        }
        private void OnDisable()
        {
            GetLobbiesList.onClick.RemoveListener(ListPublicLobbies);
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
                newLobbyInfo.GetComponentInChildren<Button>().onClick.AddListener(() => GameLobbyManager.instance.JoinLobbyById(_lobby.Id, nickName.text));
            }
        }
        public override void InputAction()
        {
            
        }
    }
}