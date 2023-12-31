using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Project3D.Controller;
using Unity.Netcode;
using Project3D.Lobbies.Manager;
using Unity.Netcode.Transports.UTP;
using Project3D.UI;
using Project3D.Lobbies;
using static UnityEngine.Rendering.DebugUI;
using System.Threading.Tasks;
using System.Threading;

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

        [SerializeField] private List<GameObject> _playerlist;
        NetworkVariable<int> standbyCount = new NetworkVariable<int>();

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

                                if(_playerlist != null)
                                {
                                    onStandbyState?.Invoke();
                                }

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
                RequestChangeClientStateServerRpc((int)value);
                Debug.Log($"[InGameManager] : changed game state ... [{value}]");
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

        [ServerRpc(RequireOwnership = false)]
        public void RequestChangeClientStateServerRpc(int newState)
        {
            ChangeStateClientRpc(newState);
        }

        [ClientRpc]
        public void ChangeStateClientRpc(int newState)
        {
            Debug.Log($"[InGameManager] : ChageStateClientRpc... to {newState}");
            LocalInGameState.instance.gameState = (GameState)newState;
        }


        [ServerRpc(RequireOwnership = false)]
        public void RequestSpawnCharacterServerRpc(ulong clientId, int characterIndex)
        {
            Debug.Log($"[InGameManager] : Spawn character... for {clientId}");
            GameObject go = Instantiate(_playerlist[characterIndex]);
            go.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        }


        private void Awake()
        {
            _instance = this;

            _players = new Dictionary<ulong, NetworkBehaviour>();

            onStandbyState += () =>
            {
                Debug.Log("check");
                //StartCoroutine(C_CheckAllReadyAndStartPlay(5.0f));
            };

            onScoreState += () =>
            {
                StartCoroutine(C_Scored());
            };

            // temp?
            onEndState += FinishGame;
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

        public Team RegisterInTeam(int teamID, ulong clientID)
        {
            RegisterInTeamServerRpc(teamID, clientID);

            return teamID == 0 ? blueTeam : redTeam;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RegisterInTeamServerRpc(int teamID, ulong clientID)
        {
            RegisterInTeamClientRpc(teamID, clientID);
        }

        [ClientRpc]
        public void RegisterInTeamClientRpc(int teamID, ulong clientID)
        {
            if (teamID == 0)
            {
                blueTeam.Register(clientID);
            }

            else
            {
                redTeam.Register(clientID);
            }
        }

        IEnumerator C_CheckAllReadyAndStartPlay(float countTimer)
        {
            Debug.Log($"[InGameManager] : Wait for all ready..");
            yield return new WaitUntil(() =>
            {
                Debug.Log($"[InGameManager] : spawned count : {CharacterControllers.spawned.Count} lobby player count : {GameLobbyManager.instance.lobbyPlayerDatas.Count}");
                return CharacterControllers.spawned.Count != GameLobbyManager.instance.lobbyPlayerDatas.Count;
            });
            StartCountDownServerRpc(countTimer);
        }

        async Task CheckAllReadyAndStartPlay(float countTimer)
        {
            Debug.Log($"[InGameManager] : Wait for all ready..");
            float timeOut = 10.0f;
            float timeMark = Time.time;
            await Task.Run(async () =>
            {
                Debug.Log("before Waiting");

                while (CharacterControllers.spawned.Count != GameLobbyManager.instance.lobbyPlayerDatas.Count &&
                       Time.time - timeMark < timeOut)
                {
                    Debug.Log("Waiting");
                    await Task.Delay(1000);
                }

                Debug.Log("checked");
                StartCountDownServerRpc(countTimer);
            });

        }

        [ServerRpc(RequireOwnership = false)]
        public void SendStandbyToServerRpc()
        {
            standbyCount.Value++;
            if (standbyCount.Value >= GameLobbyManager.instance.lobbyPlayerDatas.Count)
            {
                StartCountDownClientRpc(5.0f);
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
            Debug.Log("cd routine start");

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

        public void FinishGame(int winTeamID)
        {
            FinishGameServerRpc(winTeamID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void FinishGameServerRpc(int winTeamID)
        {
            List<ulong> winTeamPlayers = winTeamID == 0 ? _blueTeam.GetPlayersInTeam() : _redTeam.GetPlayersInTeam();

            for (int i = 0; i < winTeamPlayers.Count; i++)
            {
                _players[winTeamPlayers[i]].transform.position = _winnerSpawnPoints[i].position;
            }

            UIManager.instance.Get<UI_TopBoard>().Hide();

            FinishGameClientRpc(winTeamID);
        }

        [ClientRpc]
        public void FinishGameClientRpc(int winTeamID)
        {
            List<ulong> winTeamPlayers = winTeamID == 0 ? _blueTeam.GetPlayersInTeam() : _redTeam.GetPlayersInTeam();

            for (int i = 0; i < winTeamPlayers.Count; i++)
            {
                _players[winTeamPlayers[i]].transform.position = _winnerSpawnPoints[i].position;
            }

            UIManager.instance.Get<UI_TopBoard>().Hide();
        }
    }
}
