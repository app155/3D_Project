using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;

namespace Project3D.GameElements.Items
{
    public abstract class Item : NetworkBehaviour
    {
        [SerializeField] private LayerMask _playersMask;
        [SerializeField] private float _appearTime = 5.0f;
        [SerializeField] private float _appearTimer;

        private void OnEnable()
        {
            if (IsOwner == false)
                return;

            _appearTimer = 0.0f;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsServer == false)
                return;

            if ((1 << other.gameObject.layer & _playersMask.value) > 0)
            {
                Debug.Log($"Entered {other.gameObject.name}");

                if (other.TryGetComponent(out NetworkBehaviour target))
                {
                    Affect(target);
                }
            }
        }


        private void Update()
        {
            if (IsOwner == false)
                return;

            if (_appearTimer < _appearTime)
            {
                _appearTimer += Time.deltaTime;
            }

            else
            {
                Disappear();
            }
        }

        public abstract void Affect(NetworkBehaviour target);
        public abstract void Disappear();
    }
}
