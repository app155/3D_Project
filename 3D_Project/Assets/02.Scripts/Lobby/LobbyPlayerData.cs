using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

namespace Project3D.Lobbies
{
    public class LobbyPlayerData
    {
        public string Id { get; set; }
        public string Gametag {  get; set; }
        public bool IsReady { get; set; }

        private string _id;
        private string _gametag;
        private bool _isReady;

        public void Initialize(string id, string gametag)
        {
            _id = id;
            _gametag = gametag;
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
            if (playerData.ContainsKey("IsReady"))
            {
                _isReady = playerData["IsReady"].Value == "True";
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                { "Id", _id },
                { "Gametag", _gametag },
                { "IsReady", _isReady.ToString() }
            };
        }


    }
}