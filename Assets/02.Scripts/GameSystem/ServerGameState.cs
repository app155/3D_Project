using System;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.GameSystem
{
    public class ServerGameState : NetworkBehaviour
    {
        public static ServerGameState instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ServerGameState");
                    _instance = go.AddComponent<ServerGameState>();
                }

                return _instance;
            }
        }

        public GameState state
        {
            get => _state;
            set
            {
                if (value == _state)
                    return;

                switch (_state)
                {
                    case GameState.None:
                        break;
                    case GameState.Standby:
                        break;
                    case GameState.Playing:
                        break;
                    case GameState.Score:
                        break;
                    case GameState.End:
                        break;
                }

                _state = value;
            }
        }

        private static ServerGameState _instance;

        [SerializeField] private GameState _state;

        private void Awake()
        {
            _instance = this;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public void ChangeServerGameState(GameState state)
        {
            ChangeServerGameStateServerRpc(state);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeServerGameStateServerRpc(GameState state)
        {
            this.state = state;
        }
    }
}