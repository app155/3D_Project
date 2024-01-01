using Project3D.Controller;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Project3D.Network
{
    public class BallNetwork : NetworkBehaviour
    {
        NetworkVariable<BallNetworkData> _netState = new NetworkVariable<BallNetworkData>();
        private Vector3 _vel;
        [SerializeField] private float _interpolationTime = 0.1f;
        [SerializeField] private BallController _owner;

        public struct BallNetworkData : INetworkSerializable
        {
            private float _x, _z;

            public Vector3 Position
            {
                get => new Vector3(_x, 0.0f, _z);
                set
                {
                    _x = value.x;
                    _z = value.z;
                }
            }


            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _x);
                serializer.SerializeValue(ref _z);
            }
        }


        protected void Update()
        {
            if (IsOwner)
            {
                _netState.Value = new BallNetworkData()
                {
                    Position = transform.position,
                };
            }

            else
            {
                Vector3 vel = _owner.moveDir * _owner.moveSpeed;
                transform.position = Vector3.SmoothDamp(transform.position, _netState.Value.Position, ref vel, _interpolationTime);
            }
        }
    }
}