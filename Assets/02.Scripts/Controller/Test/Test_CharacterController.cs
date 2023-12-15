/*//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Netcode;

<<<<<<< HEAD
//public class CharacterController : NetworkBehaviour
//{
//    public float speed
//    {
//        get => _speed;
//    }

//    [SerializeField] private float _speed;
//    [SerializeField] LayerMask _groundMask;
//    private Rigidbody _rigid;
=======
namespace 3Dproject.Controllers
{
    public enum State
  {
    Idle,
    Shoot,
    Move,
    Skill1,
    Skill2,
    Skill3,
 }
 public class Test_CharacterController : NetworkBehaviour
 {


    public float speed
    {
        get => _speed;
        set 
        { 
            _speed = value; 
        }
    }

    public int hp
    {
        get => _hp.Value;
        set
        {
            _hp.Value = value;
        }
    }

    public int hpMax => _hpMax.Value;

    [SerializeField] private float _speed;
    [SerializeField] LayerMask _groundMask;
    private Rigidbody _rigid;
    private NetworkVariable<int> _hp = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> _hpMax = new NetworkVariable<int>(100);
    public event Action<int> onHpChanged;
    public State state;
    private NetworkVariable<float> _horizontal = new NetworkVariable<float>(0.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> _vertical = new NetworkVariable<float>(0.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> _moveGain = new NetworkVariable<float>(0.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Vector3 _velocity;
    private Vector3 _accel;
>>>>>>> MS

//    public override void OnNetworkSpawn()
//    {
//        if (IsOwner == false)
//        {
//            enabled = false;
//        }

<<<<<<< HEAD
//        base.OnNetworkSpawn();
=======
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            _hp.Value = hpMax;
        }
        _hp.OnValueChanged += (prev, current) => onHpChanged?.Invoke(current);
>>>>>>> MS

//        _rigid = GetComponent<Rigidbody>();
//    }

//    private void Update()
//    {
//        if (IsGrounded())
//        {
//            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
//        }
//    }

//    private void FixedUpdate()
//    {
//        if (IsOwner == false)
//            return;

//        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")) * Time.fixedDeltaTime;
//    }

<<<<<<< HEAD
//    private bool IsGrounded()
//    {
//        Collider[] cols = Physics.OverlapSphere(transform.position, 0.15f, _groundMask);

//        return cols.Length > 0;
//    }
//}
=======
    private bool IsGrounded()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 0.3f, _groundMask);

        return cols.Length > 0;
    }
    private void Hit() { }

 }
}
>>>>>>> MS
*/