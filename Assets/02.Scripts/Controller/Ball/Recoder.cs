using Project3D.GameSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class Recoder : NetworkBehaviour
    {
        private Queue<ulong> _hits;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _hits = new Queue<ulong>(30);
        }

        public void Add(ulong hit = 1111)
        {
            _hits.Enqueue(hit);
        }

        public ulong GetScorer(int scoreTeamID)
        {
            ulong scorer = 1111;

            while (_hits.Count > 0)
            {
                ulong hit = _hits.Dequeue();

                if (InGameManager.instance.player[hit].TryGetComponent(out CharacterControllers scoreChara))
                {
                    if (scoreChara.team.id == scoreTeamID)
                    {
                        scorer = hit;
                    }
                }

                else
                {
                    Debug.Log("Cannot find Scorer");
                }
            }

            return scorer;
        }
    }
}