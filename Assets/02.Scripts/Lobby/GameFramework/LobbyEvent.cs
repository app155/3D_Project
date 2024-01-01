using Unity.Services.Lobbies.Models;

namespace Project3D.Lobbies.GameFramework
{
    public static class LobbyEvent
    {
        public delegate void LobbyUpdated();
        public static LobbyUpdated OnLobbyUpdated;

        public delegate void LobbyReady();
        public static LobbyReady OnLobbyReady;
    }
}