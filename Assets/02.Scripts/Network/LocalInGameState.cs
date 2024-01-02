using Project3D.GameSystem;
using Project3D.Lobbies;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class LocalInGameState :  MonoBehaviour
{
    public static LocalInGameState instance;
    private void Awake()
    {
        instance = this;
    }
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
                        if (InGameManager.instance.player.ContainsKey(NetworkManager.Singleton.LocalClientId) == false)
                            InGameManager.instance.RequestSpawnCharacterServerRpc(NetworkManager.Singleton.LocalClientId, GameLobbyManager.instance.LocalLobbyPlayerData.Character);

                        InGameManager.instance.SendStandbyToServerRpc();
                    }
                    break;
                case GameState.Playing:
                    {
                    }
                    break;
                case GameState.Score:
                    {
                    }
                    break;
                case GameState.End:
                    {
                    }
                    break;
            }
            Debug.Log($"[LocalInGameState] : changed game state ... [{value}]");
            _gameState = value;
        }
    }
    [SerializeField] private GameState _gameState;
}
