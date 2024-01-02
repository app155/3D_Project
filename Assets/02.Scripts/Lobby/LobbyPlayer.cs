using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Project3D.Lobbies
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TextMeshProUGUI _isReadyRenderer;
        [SerializeField] private Image _teamColor;

        private LobbyPlayerData _data;

        
        private void Start()
        {
        }
        public void SetData(LobbyPlayerData data)
        {

            _data = data;
            _playerName.text = _data.NickName;
            if(_data.Team == 1)
            {
                _teamColor.color = Color.blue;
            }
            else
            {
                _teamColor.color = Color.red;
            }

            if (_data.IsReady)
            {
                if(_isReadyRenderer != null)
                {
                    _isReadyRenderer.gameObject.SetActive(true);
                }
                
            }
            gameObject.SetActive(true);
        }
    }
}