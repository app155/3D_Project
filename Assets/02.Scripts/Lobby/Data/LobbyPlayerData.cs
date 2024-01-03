using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Project3D.Lobbies
{
    public class LobbyPlayerData
    {
        public string Id => _id;
        public string Gametag => _gametag;
        public bool IsReady
        {
            get => _isReady;
            set => _isReady = value;
        }
        public string NickName => _nickName;

        public int Team
        {
            get => _team;
            set => _team = value;
        }
        public int Character
        {
            get => _character;
            set => _character = value;
        }
        private int _team;
        private int _character;
        private string _nickName;
        private string _id;
        private string _gametag;
        private bool _isReady;

        public void Initialize(string id, string gametag, string nickName)
        {
            _id = id;
            _gametag = gametag;
            _nickName = nickName;
            _team = 0;
            _character = 0;
        }

        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if(playerData.ContainsKey("Id"))
            {
                _id = playerData["Id"].Value;
            }
            if (playerData.ContainsKey("Gametag"))
            {
                _gametag = playerData["Gametag"].Value;

            }
            if (playerData.ContainsKey("NickName"))
            {
                _nickName = playerData["NickName"].Value;
            }
            if (playerData.ContainsKey("IsReady"))
            {
                _isReady = playerData["IsReady"].Value == "True";
            }
            if (playerData.ContainsKey("Team"))
            {
                _team = int.Parse(playerData["Team"].Value);
            }
            if (playerData.ContainsKey("Character"))
            {
                _character = int.Parse(playerData["Character"].Value);
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                { "Id", _id },
                { "Gametag", _gametag },
                { "IsReady", _isReady.ToString() },
                { "NickName", _nickName },
                { "Team", _team.ToString() },
                {"Character", _character.ToString() }
            };
        }


    }
}