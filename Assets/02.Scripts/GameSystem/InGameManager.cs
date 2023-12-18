using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Project3D.Controller;
using Unity.Netcode;

namespace Project3D.GameSystem
{
    public enum GameState
    {
        None,
        Standby,
        Playing,
        Score,
        End,
    }

    public class InGameManager : NetworkBehaviour
    {
        public static InGameManager instance => _instance;

        public Dictionary<ulong, NetworkBehaviour> player => _players;

        public GameState gameState
        {
            get => _gameState;
            set
            {
                if (IsServer == false)
                    return;

                if (_gameState == value)
                    return;

                switch (value)
                {
                    case GameState.None:
                        break;
                    case GameState.Standby:
                        {
                            onStandbyState?.Invoke();
                        }
                        break;
                    case GameState.Playing:
                        {
                            onPlayingState?.Invoke();
                        }
                        break;
                    case GameState.Score:
                        {
                            onScoreState?.Invoke();
                        }
                        break;
                    case GameState.End:
                        {
                            onEndState?.Invoke();
                        }
                        break;
                }

                _gameState = value;
            }
        }

        public event Action onStandbyState;
        public event Action onPlayingState;
        public event Action onScoreState;
        public event Action onEndState;

        private static InGameManager _instance;

        [SerializeField] private GameState _gameState;

        private Dictionary<ulong, NetworkBehaviour> _players;
        private Team _blueTeam;
        private Team _redTeam;

        private void Awake()
        {
            _instance = this;

            _players = new Dictionary<ulong, NetworkBehaviour>();

            onScoreState += () =>
            {
                Debug.Log($"{OwnerClientId} call onscoreState");
                StartCoroutine(C_Scored());
            };
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        IEnumerator C_Scored()
        {
            yield return new WaitForSeconds(1.0f);

            gameState = GameState.Standby;
        }

        public void RegisterPlayer(ulong clientID, NetworkBehaviour player)
        {
            if (_players.TryAdd(clientID, player) == false)
            {
                throw new Exception("[InGameManager] - Register");
            }
        }
    }
}
