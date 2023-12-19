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

        public event Action<float> onCountdownChanged;

        private static InGameManager _instance;

        [SerializeField] private GameState _gameState;

        private Dictionary<ulong, NetworkBehaviour> _players;
        private Team _blueTeam;
        private Team _redTeam;

        private void Awake()
        {
            _instance = this;

            _players = new Dictionary<ulong, NetworkBehaviour>();

            onStandbyState += () =>
            {
                StartCountDownServerRpc(5.0f);
            };

            onScoreState += () =>
            {
                StartCoroutine(C_Scored());
            };
        }

        private void Start()
        {

        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log("Ingamemanager spawned");

            ChangeGameStateServerRpc(GameState.Standby);
        }

        IEnumerator C_Scored()
        {
            yield return new WaitForSeconds(1.0f);

            ChangeGameStateServerRpc(GameState.Standby);
        }

        public void RegisterPlayer(ulong clientID, NetworkBehaviour player)
        {
            if (_players.TryAdd(clientID, player) == false)
            {
                throw new Exception("[InGameManager] - Register");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void StartCountDownServerRpc(float countTimer)
        {
            StartCountDownClientRpc(countTimer);
        }

        [ClientRpc]
        public void StartCountDownClientRpc(float countTimer)
        {
            StartCoroutine(C_StartCountDown(countTimer));
        }

        IEnumerator C_StartCountDown(float countTimer)
        {
            onCountdownChanged?.Invoke(countTimer);
            Debug.Log(countTimer);
            float start = Time.time;

            while (countTimer > 0f)
            {
                float now = Time.time;

                if (now - start > 1.0f)
                {
                    countTimer -= 1.0f;
                    start = now;
                    onCountdownChanged?.Invoke(countTimer);
                    Debug.Log(countTimer);
                }

                yield return null;
            }

            Debug.Log("End startcountdown coroutine");
            ChangeGameStateServerRpc(GameState.Playing);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeGameStateServerRpc(GameState state)
        {
            ChangeGameStateClientRpc(state);
        }

        [ClientRpc]
        public void ChangeGameStateClientRpc(GameState state)
        {
            gameState = state;
        }
    }
}
