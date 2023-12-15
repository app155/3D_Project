using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterController;

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
            if (IsOwner == false)
                return;

            if ((1 << other.gameObject.layer & _playersMask.value) > 0)
            {
                Debug.Log("Entered");
                Affect(other.transform);
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
                gameObject.SetActive(false);
            }
        }

        public abstract void Affect(Transform target);
    }
}
