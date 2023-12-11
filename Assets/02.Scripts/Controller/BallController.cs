using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class BallController : NetworkBehaviour
    {
        [SerializeField] private Vector3 _moveDir;
        [SerializeField] private LayerMask _characterMask;
        [SerializeField] private LayerMask _wallMask;
        [SerializeField] private Vector3 _pushDir;
        [SerializeField] private float _pushPower;
        private Rigidbody rigid;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector3 _moveStartPos;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _characterMask) > 0)
            {
                Debug.Log($"{other.gameObject.name} triggered");

                Vector3 otherPos = new Vector3(other.transform.position.x, 0.0f, other.transform.position.z);

                Vector3 tempDir = transform.position - otherPos;
                tempDir = new Vector3(tempDir.x, 0.0f, tempDir.z);

                _moveDir = tempDir.normalized;

                _moveSpeed = _pushPower;

                _moveStartPos = transform.position;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((1 << collision.gameObject.layer & _wallMask) > 0)
            {
                Vector3 tempVec = collision.contacts[0].normal;
                Vector3 normalVec = new Vector3(tempVec.x, 0.0f, tempVec.z);

                Vector3 tempDir = Vector3.Reflect(_moveDir, normalVec).normalized;

                _moveDir = new Vector3(tempDir.x, 0.0f, tempDir.z);

            }
        }

        private void Update()
        {
            if (_moveSpeed > 0.0f)
                _moveSpeed -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            rigid.position += _moveDir * _moveSpeed * Time.fixedDeltaTime;
        }
    }
}