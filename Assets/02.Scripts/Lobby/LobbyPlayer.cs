using UnityEngine;
using TMPro;

namespace Project3D.Lobbies
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _isReadyRenderer;

        private LobbyPlayerData _data;

        private void Start()
        {
        }
        public void SetData(LobbyPlayerData data)
        {

            _data = data;
            _playerName.text = _data.NickName;
            if (_data.IsReady)
            {
                if(_isReadyRenderer != null)
                {
                    _isReadyRenderer.gameObject.SetActive(true);
                }
                
            }
            Debug.Log($"{data.Gametag}");
            gameObject.SetActive(true);
        }
    }
}