using Project3D.GameSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class BallController : NetworkBehaviour, IKnockback
    {
        public float moveSpeed => _moveSpeed;
        public Vector3 moveDir => _moveDir;

        [SerializeField] private Vector3 _moveDir;
        [SerializeField] private LayerMask _characterMask;
        [SerializeField] private LayerMask _wallMask;
        [SerializeField] private LayerMask _lockerMask;
        private Rigidbody _rigid;
        private CapsuleCollider _col;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector3 _moveStartPos;

        private void Start()
        {
            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _rigid = GetComponent<Rigidbody>();
            _col = GetComponent<CapsuleCollider>();
            InGameManager.instance.onStandbyState += ResetServerRpc;
            Debug.Log("ball spawned");
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

            Collider[] bounces = Physics.OverlapSphere(transform.position + _moveDir * _moveSpeed * Time.fixedDeltaTime,
                                                       _col.radius,
                                                       _wallMask);


            if (bounces.Length > 0)
            {
                Collider wall = bounces[0];

                for (int i = 1; i < bounces.Length; i++)
                {
                    if (Vector3.Distance(bounces[i].transform.position, transform.position) < Vector3.Distance(wall.transform.position, transform.position))
                    {
                        wall = bounces[i];
                    }
                }

                Vector3 normalVec;
                Vector3 normalVecWithRight = wall.transform.TransformDirection(Vector3.right);
                Vector3 normalVecWithForward = wall.transform.TransformDirection(Vector3.forward);

                //if (wall.transform.rotation.eulerAngles.y == 0)
                //{
                //    Vector3 contactPos = wall.ClosestPointOnBounds(transform.position);
                //    normalVec = (transform.position - contactPos).normalized;
                //}

                //else
                //{
                //    normalVec = wall.transform.TransformDirection(Vector3.right);
                //}

                Debug.Log($"bounceBefore = {_moveDir}");
                Vector3 reflectVecWithRight = Vector3.Reflect(_moveDir, normalVecWithRight).normalized;
                Vector3 reflectVecWithForward = Vector3.Reflect(_moveDir, normalVecWithForward).normalized;

                Collider[] afterReflect = Physics.OverlapSphere(transform.position + reflectVecWithRight * _moveSpeed * Time.fixedDeltaTime, _col.radius, _wallMask);

                _moveDir = afterReflect.Length > 0 ? reflectVecWithForward : reflectVecWithRight;

                Debug.Log($"bounceAfter = {_moveDir}");

                if (wall.TryGetComponent(out IBounce executer))
                {
                    executer.Execute();
                }
            }
        }




        [ServerRpc(RequireOwnership = false)]
        public void KnockbackServerRpc(Vector3 pushDir, float pushPower, ServerRpcParams rpcParams = default)
        {
            _moveDir = pushDir;
            _moveSpeed = pushPower;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ScoreServerRpc()
        {
            InGameManager.instance.gameState = GameState.Score;
            ScoreClientRpc();
        }

        [ClientRpc]
        public void ScoreClientRpc()
        {
            gameObject.SetActive(false);
            _moveDir = Vector3.zero;
            _moveSpeed = 0.0f;
            transform.position = Vector3.zero + Vector3.up * 0.1f;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ResetServerRpc()
        {
            ResetClientRpc();
        }

        [ClientRpc]
        public void ResetClientRpc()
        {
            gameObject.SetActive(true);
        }
    }
}