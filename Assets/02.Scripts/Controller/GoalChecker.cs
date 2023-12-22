using Project3D.Controller;
using Project3D.GameSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class GoalChecker : NetworkBehaviour
    {
        public Team team => _teamEnum == Teams.Blue ? InGameManager.instance.blueTeam : InGameManager.instance.redTeam;

        [SerializeField] private LayerMask _ballMask;
        [SerializeField] private Teams _teamEnum;

        [SerializeField] private int _winningScore;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _ballMask) > 0)
            {
                other.GetComponent<BallController>().ScoreServerRpc(team.id);
                ScoreServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ScoreServerRpc()
        {
            ScoreClientRpc();
        }

        [ClientRpc]
        public void ScoreClientRpc()
        {
            team.score++;

            InGameManager.instance.ChangeGameStateClientRpc(GameState.Score);
        }
    }
}
