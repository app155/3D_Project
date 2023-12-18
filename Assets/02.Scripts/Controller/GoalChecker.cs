using Project3D.Controller;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class GoalChecker : NetworkBehaviour
    {
        [SerializeField] private LayerMask _ballMask;

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _ballMask) > 0)
            {
                other.GetComponent<BallController>().ScoreServerRpc();
            }
        }
    }
}
