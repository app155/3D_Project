using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class GoalLocker : NetworkBehaviour
    {
        public int knockCount
        {
            get => _knockCount;
            set
            {
                if (IsOwner == false)
                    return;

                _knockCount = value;

                if (_knockCount <= 0)
                {
                    DisapearServerRpc();
                }
            }
        }

        [SerializeField] private int _knockCount;
        [SerializeField] private int _knockCountOffset = 1;

        [ServerRpc(RequireOwnership = false)]
        public void ResetGoalLockerServerRpc()
        {
            ResetGoalLockerClientRpc();
        }

        [ClientRpc]
        public void ResetGoalLockerClientRpc()
        {
            knockCount = _knockCountOffset;
        }

        [ServerRpc(RequireOwnership = false)]
        public void DisapearServerRpc()
        {
            DisapearClientRpc();
        }

        [ClientRpc]
        public void DisapearClientRpc()
        {
            gameObject.SetActive(false);
        }

    }
}
