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
        [SerializeField] private LayerMask _groundMask;
        private Rigidbody _rigid;
        private CapsuleCollider _col;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector3 _moveStartPos;

        private Recoder _recoder;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _rigid = GetComponent<Rigidbody>();
            _col = GetComponent<CapsuleCollider>();
            InGameManager.instance.onStandbyState += ResetServerRpc;
            Debug.Log("ball spawned");
            _recoder = GetComponentInChildren<Recoder>();
        }


        private void Update()
        {
            if (!IsOwner)
                return;

            if (_moveSpeed > 0.0f)
                _moveSpeed -= Time.deltaTime;

            else if (_moveSpeed < 0.0f)
                _moveSpeed = 0.0f;

            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, _groundMask) == false)
            {
                transform.position = Vector3.zero;
                _moveSpeed = 0.0f;
                return;
            }

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

                Vector3 normalVecWithRight = wall.transform.TransformDirection(Vector3.right);
                Vector3 normalVecWithForward = wall.transform.TransformDirection(Vector3.forward);

                Vector3 reflectVecWithRight = Vector3.Reflect(_moveDir, normalVecWithRight).normalized;
                Vector3 reflectVecWithForward = Vector3.Reflect(_moveDir, normalVecWithForward).normalized;

                Collider[] afterReflect = Physics.OverlapSphere(transform.position + reflectVecWithRight * _moveSpeed * Time.fixedDeltaTime, _col.radius, _wallMask);

                _moveDir = afterReflect.Length > 0 ? reflectVecWithForward : reflectVecWithRight;


                if (wall.TryGetComponent(out IBounce executer))
                {
                    executer.Execute();
                }
            }
        }

        private void FixedUpdate()
        {
            if (!IsOwner)
                return;

            transform.position += _moveDir * _moveSpeed * Time.fixedDeltaTime;
        }

        [ServerRpc(RequireOwnership = false)]
        public void KnockbackServerRpc(Vector3 pushDir, float pushPower, ulong clientID, ServerRpcParams rpcParams = default)
        {
            _moveDir = pushDir;
            _moveSpeed = pushPower;
            InGameManager.instance.player[clientID].GetComponent<CharacterControllers>().expBar.IncreaseExpServerRpc((int)Formulas.CalcExp(1f, 1));
            Debug.Log(clientID);
            _recoder.Add(clientID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ScoreServerRpc(int teamID, ServerRpcParams rpcParams = default)
        {
            ulong scorerID = _recoder.GetScorer(teamID);

            ScoreClientRpc(teamID, scorerID);
        }

        [ClientRpc]
        public void ScoreClientRpc(int teamID, ulong scorerID, ClientRpcParams rpcParams = default)
        {
            gameObject.SetActive(false);
            _moveDir = Vector3.zero;
            _moveSpeed = 0.0f;
            transform.position = Vector3.zero + Vector3.up * 0.1f;
            Debug.Log($"scorer = {scorerID}");
            InGameManager.instance.scorerID = scorerID;
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