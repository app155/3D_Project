using UnityEngine;
using TMPro;

namespace Project3D.Lobbies
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _playerName;

        private LobbyPlayerData _data;
        public void SetData(LobbyPlayerData data)
        {
            _data = data;
            _playerName.text = _data.Gametag;
            gameObject.SetActive(true);
        }
    }
}