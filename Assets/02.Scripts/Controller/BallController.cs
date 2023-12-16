using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class BallController : NetworkBehaviour, IKnockback
    {
        [SerializeField] private Vector3 _moveDir;
        [SerializeField] private LayerMask _characterMask;
        [SerializeField] private LayerMask _wallMask;
        [SerializeField] private Vector3 _pushDir;
        [SerializeField] private float _pushPower;
        private Rigidbody _rigid;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector3 _moveStartPos;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _rigid = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsOwner)
                return;


            if ((1 << other.gameObject.layer & _wallMask) > 0)
            {
                Debug.Log($"{other.gameObject.name} triggered");
                
                Vector3 tempPos = other.ClosestPointOnBounds(transform.position);


                // Temp Reflect -> Need to Modify with Wall
                Vector3 wallsurfaceDirRight = other.transform.TransformDirection(Vector3.right);
                Vector3 wallsurfaceDirFoward = other.transform.TransformDirection(Vector3.forward);

                Vector3 normalVec = (transform.position - tempPos).normalized;
                Debug.Log($"normalVec = {normalVec}");

                Vector3 tempDir;

                if (other.transform.rotation.y != 0)
                    tempDir = Vector3.Reflect(_moveDir, wallsurfaceDirRight).normalized;

                else
                    tempDir = Vector3.Reflect(_moveDir, normalVec).normalized;


                _moveDir = new Vector3(tempDir.x, 0.0f, tempDir.z);
            }
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            if (_moveSpeed > 0.0f)
                _moveSpeed -= Time.deltaTime;

            else if (_moveSpeed < 0.0f)
                _moveSpeed = 0.0f;
        }

        private void FixedUpdate()
        {
            if (!IsOwner)
                return;

            _rigid.position += _moveDir * _moveSpeed * Time.fixedDeltaTime;
        }



        [ServerRpc(RequireOwnership = false)]
        public void KnockbackServerRpc(Vector3 pushDir, float pushPower, ServerRpcParams rpcParams = default)
        {
            _moveDir = pushDir;
            _moveSpeed = pushPower;
        }
    }
}