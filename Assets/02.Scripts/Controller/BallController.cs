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

            // Ä³¸¯ÅÍ Á¢ÃË ½Ã Æ¨±â±â - ¾È¾¸
            //if ((1 << other.gameObject.layer & _characterMask) > 0)
            //{
            //    Debug.Log($"{other.gameObject.name} triggered");

            //    Vector3 otherPos = new Vector3(other.transform.position.x, 0.0f, other.transform.position.z);

            //    Vector3 tempDir = transform.position - otherPos;
            //    tempDir = new Vector3(tempDir.x, 0.0f, tempDir.z);

            //    _moveDir = tempDir.normalized;

            //    _moveSpeed = _pushPower;

            //    _moveStartPos = transform.position;
            //}

            if ((1 << other.gameObject.layer & _wallMask) > 0)
            {
                Debug.Log($"{other.gameObject.name} triggered");
                
                Vector3 tempPos = other.ClosestPointOnBounds(transform.position);
                Vector3 normalVec = (transform.position - tempPos).normalized;
                Vector3 normalVec2 = other.transform.rotation * Vector3.up;

                float angle = Vector3.Angle(normalVec2, _moveDir);
                Vector3 tempDir = new Vector3();

                if (angle <= 45.0f)
                    tempDir = Vector3.Reflect(_moveDir, normalVec);

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