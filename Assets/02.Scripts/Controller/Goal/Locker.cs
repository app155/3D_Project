using Project3D.GameSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class Locker : NetworkBehaviour
    {
        [SerializeField] private GameObject[] _doors;
        [SerializeField] private GoalLocker[] _lockers;

        [SerializeField] private GameObject _upDoor;
        [SerializeField] private GameObject _downDoor;
        [SerializeField] private GoalLocker _upLocker;
        [SerializeField] private GoalLocker _downLocker;

        private Vector3 _upDoorOriginPos;
        private Vector3 _downDoorOriginPos;

        private bool _isDoorOpening;
        [SerializeField] private float _doorMoveSpeed;

        private void Awake()
        {
            (_upDoor, _downDoor) = _doors[0].transform.position.z > _doors[1].transform.position.z ? (_doors[0], _doors[1]) : (_doors[1], _doors[0]);
            (_upLocker, _downLocker) = _lockers[0].transform.position.z > _lockers[1].transform.position.z ? (_lockers[0], _lockers[1]) : (_lockers[1], _lockers[0]);
        }

        private void Start()
        {
            InGameManager.instance.onStandbyState += ResetObject;
        }

        private void OnEnable()
        {
            _upDoorOriginPos = _upDoor.transform.position;
            _downDoorOriginPos = _downDoor.transform.position;
        }

        private void Update()
        {
            if (IsServer == false)
                return;

            if (_isDoorOpening == false && _upLocker.knockCount == 0 && _downLocker.knockCount == 0)
            {
                _isDoorOpening = true;
                ChangeDoorMoveSpeedServerRpc(1.0f);
            }

            if (Vector3.Distance(_upDoor.transform.position, _upDoorOriginPos) >= 3)
            {
                DisappearDoorServerRpc();
            }
        }

        private void FixedUpdate()
        {
            if (IsServer == false)
                return;

            _upDoor.transform.position += Vector3.forward * _doorMoveSpeed * Time.fixedDeltaTime;
            _downDoor.transform.position += Vector3.back * _doorMoveSpeed * Time.fixedDeltaTime;
        }

        public void ResetObject()
        {
            if (IsServer == false)
                return;

            ResetObjectServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void ResetObjectServerRpc()
        {
            ResetObjectClientRpc();
        }

        [ClientRpc]
        public void ResetObjectClientRpc()
        {
            foreach (GoalLocker locker in _lockers)
                locker.ResetGoalLockerServerRpc();

            AppearDoorServerRpc();

            _isDoorOpening = false;
            _doorMoveSpeed = 0.0f;
            _upDoor.transform.position = _upDoorOriginPos;
            _downDoor.transform.position = _downDoorOriginPos;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeDoorMoveSpeedServerRpc(float speed)
        {
            _doorMoveSpeed = speed;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AppearDoorServerRpc()
        {
            AppearDoorClientRpc();
        }

        [ClientRpc]
        public void AppearDoorClientRpc()
        {
            _upDoor.SetActive(true);
            _downDoor.SetActive(true);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DisappearDoorServerRpc()
        {
            DisappearDoorClientRpc();
        }

        [ClientRpc]
        public void DisappearDoorClientRpc()
        {
            _upDoor.SetActive(false);
            _downDoor.SetActive(false);
        }
    }
}
