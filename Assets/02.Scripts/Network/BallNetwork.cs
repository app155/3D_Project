using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallNetwork : NetworkBehaviour
{
    NetworkVariable<BallNetworkData> _netState = new NetworkVariable<BallNetworkData>(writePerm: NetworkVariableWritePermission.Owner);
    private Vector3 _vel;
    [SerializeField] private float _interpolationTime = 0.1f;

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

    private void Update()
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
            transform.position = Vector3.SmoothDamp(transform.position, _netState.Value.Position, ref _vel, _interpolationTime);
        }
    }
}
