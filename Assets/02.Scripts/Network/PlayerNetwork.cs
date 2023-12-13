using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    NetworkVariable<PlayerNetworkData> _netState = new NetworkVariable<PlayerNetworkData>(writePerm: NetworkVariableWritePermission.Owner);
    private Vector3 _vel;
    private float _rotVel;
    [SerializeField] private float _interpolationTime = 0.1f;

    public struct PlayerNetworkData : INetworkSerializable
    {
        private float _x, _z;
        private short _y;

        public Vector3 Position
        {
            get => new Vector3(_x, 0.0f, _z);
            set
            {
                _x = value.x;
                _z = value.z;
            }
        }

        public Vector3 Rotation
        {
            get => new Vector3(0.0f, _y, 0.0f);
            set => _y = (short)value.y;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _x);
            serializer.SerializeValue(ref _y);
            serializer.SerializeValue(ref _z);
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            _netState.Value = new PlayerNetworkData()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles
            };
        }

        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, _netState.Value.Position, ref _vel, _interpolationTime);
            transform.rotation = Quaternion.Euler(0,
                                                  Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y,
                                                                        _netState.Value.Rotation.y,
                                                                        ref _rotVel,
                                                                        _interpolationTime),
                                                  0);
        }
    }
}
