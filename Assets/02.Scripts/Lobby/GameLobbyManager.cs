using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Project3D.Lobbies
{
    public class GameLobbyManager : MonoBehaviour
    {
        public static GameLobbyManager instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("LobbyManager");
                    _instance = go.AddComponent<GameLobbyManager>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private static GameLobbyManager _instance;

        public async Task<bool> CreateLobby()
        {
            Dictionary<string, string> playerData = new Dictionary<string, string>()
            {
                { "GamerTag", "HostPlayer" }
            };

            bool succeeded = await LobbyManager.instance.CreateLobby(4, true, playerData);

            return succeeded;
        }
    }
}
