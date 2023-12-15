using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Project3D.Controller;
using Unity.Netcode;

namespace Project3D.GameSystem
{
    public class InGameManager : NetworkBehaviour
    {
        public static InGameManager instance;
        public Dictionary<ulong, NetworkBehaviour> player => _players;

        private Dictionary<ulong, NetworkBehaviour> _players;

        private void Awake()
        {
            instance = this;

            _players = new Dictionary<ulong, NetworkBehaviour>();
        }

        public void RegisterPlayer(NetworkBehaviour player)
        {
            if (_players.TryAdd(player.OwnerClientId, player) == false)
            {
                throw new Exception("[InGameManager] - Register");
            }
        }
    }
}
