using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_BallController : MonoBehaviour
{
    [SerializeField] private Vector3 _moveDir;
    [SerializeField] private LayerMask _targetLayer;
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
        if ((1 << other.gameObject.layer & _targetLayer) > 0)
        {
            Debug.Log($"{other.gameObject.name} triggered");

            if (_moveSpeed == 0f)
            {
                Vector3 otherPos = new Vector3(other.transform.position.x, 0.0f, other.transform.position.z);

                Vector3 tempDir = transform.position - otherPos;
                tempDir = new Vector3(tempDir.x, 0.0f, tempDir.z);

                _moveDir = tempDir.normalized;

                _moveSpeed = _pushPower;
            }

            else
            {
            
            }
            
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
