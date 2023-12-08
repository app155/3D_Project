using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Test_CharacterController : NetworkBehaviour
{
    public float speed
    {
        get => _speed;
    }

    [SerializeField] private float _speed;
    [SerializeField] LayerMask _groundMask;
    private Rigidbody _rigid;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (IsGrounded())
        {
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
    }

    private bool IsGrounded()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 0.15f, _groundMask);

        return cols.Length > 0;
    }
}