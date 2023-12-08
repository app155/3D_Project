using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_BallController : MonoBehaviour
{
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private LayerMask _characterMask;
    [SerializeField] private LayerMask _wallMask;
    [SerializeField] private Vector3 _pushDir;
    [SerializeField] private float _pushPower;
    private Rigidbody rigid;
    [SerializeField] private float _moveSpeed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        //rigid.AddForce(_pushDir.normalized * _pushPower, ForceMode.Impulse);
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

            _moveSpeed = _pushPower;
        }
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        transform.position += _moveDir * _moveSpeed * Time.fixedDeltaTime;
    }
}
