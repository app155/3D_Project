using Unity.Services.Lobbies.Models;

namespace Project3D.Lobbies
{
    public static class LobbyEvent
    {
        public delegate void LobbyUpdated(Lobby lobby);
        public static LobbyUpdated OnLobbyUpdated;

    }
}