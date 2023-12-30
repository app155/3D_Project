using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Project3D.Controller;
using Unity.Netcode;
using Project3D.Lobbies.Manager;
using Unity.Netcode.Transports.UTP;
using Project3D.UI;

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

                if (IsOwner)
                {
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
                                onEndState?.Invoke(Convert.ToInt32(_redTeam.score == _winningPoint));
                            }
                            break;
                    }
                }

                _gameState = value;
            }
        }

        public event Action onStandbyState;
        public event Action onPlayingState;
        public event Action onScoreState;
        public event Action<int> onEndState;
        public event Action<float> onCountdownChanged;

        public Team blueTeam => _blueTeam;
        public Team redTeam => _redTeam;
        public Transform[] spawnPoints => _spawnPoints;

        public ulong scorerID;

        private static InGameManager _instance;

        [SerializeField] private GameState _gameState;

        private Dictionary<ulong, NetworkBehaviour> _players;
        private Team _blueTeam = new Team(0);
        private Team _redTeam = new Team(1);
        [SerializeField] private int _winningPoint;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private Transform[] _winnerSpawnPoints;

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

            // temp?
            onEndState += (value) =>
            {
                List<ulong> winTeamPlayers = _blueTeam.score == _winningPoint ? _blueTeam.GetPlayersInTeam() : _redTeam.GetPlayersInTeam();

                for (int i = 0; i <  winTeamPlayers.Count; i++)
                {
                    _players[winTeamPlayers[i]].transform.position = _winnerSpawnPoints[i].position;
                }

                UIManager.instance.Get<UI_TopBoard>().Hide();
            };
        }

        private void Start()
        {            
            NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
            if (RelayManager.Instance.IsHost)
            {
                NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionApproval;
                (byte[] allocationId, byte[] key, byte[] connectionData, string ip, int port) = RelayManager.Instance.GetHostConnectionInfo();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ip, (ushort)port, allocationId, key, connectionData, true);
                NetworkManager.Singleton.StartHost();
            }

            else
            {
                (byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, string ip, int port) = RelayManager.Instance.GetClientConnectionInfo();
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ip, (ushort)port, allocationId, key, connectionData, hostConnectionData, true);
                NetworkManager.Singleton.StartClient();
            }
        }

        private void ConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Pending = false;
            response.CreatePlayerObject = true;
            response.Approved = true;

        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner == false)
            {
                RequestCurrentGameStateServerRpc();
            }

            else
            {
                ChangeGameStateServerRpc(GameState.Standby);

                Debug.Log("Ingamemanager spawned");
            }
        }

        IEnumerator C_Scored()
        {
            yield return new WaitForSeconds(5.0f);

            if (_blueTeam.score >= _winningPoint || _redTeam.score >= _winningPoint)
            {
                ChangeGameStateServerRpc(GameState.End);
            }

            else
            {
                ChangeGameStateServerRpc(GameState.Standby);
            }
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
            float start = Time.time;

            while (countTimer > -1f)
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
        public void RequestCurrentGameStateServerRpc()
        {
            RespondCurrentGameStateClientRpc(gameState);
        }

        [ClientRpc]
        public void RespondCurrentGameStateClientRpc(GameState curState)
        {
            if (IsServer)
                return;

            gameState = curState;
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
