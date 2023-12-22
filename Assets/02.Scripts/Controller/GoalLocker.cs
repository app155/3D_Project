using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class GoalLocker : NetworkBehaviour, IBounce
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
            gameObject.SetActive(true);
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
            Debug.Log("disapear");
        }

        public void Execute()
        {
            knockCount--;
        }
    }
}
