using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Project3D.GameElements.Skill;

namespace Project3D.Controller
{
    public class CharacterController : NetworkBehaviour, IHp, IKnockback
    {
        public enum CharacterState
        {
            None,
            Locomotive,
            Respawned,
            Attack,
            Hit,
            Ceremony,
            Die,
        }

        public float speed
        {
            get => _speed;
        }

        public float HpValue
        {
            get => _hpValue;
            set
            {
                if (_hpValue == value)
                    return;

                _hpValue = Mathf.Clamp(value, _hpMin, _hpMax);
            }
        }

        public float HpMax
        {
            get => _hpMax;
            set => _hpMax = value;
        }

        public float xAxis
        {
            get => _xAxis;
            set => _xAxis = value;
        }

        public float zAxis
        {
            get => _zAxis;
            set => _zAxis = value;
        }

        public LayerMask enemyMask => _enemyMask;
        public LayerMask ballMask => _ballMask;
        public LayerMask groundMask => _groundMask;
        public float HpMin => _hpMin;

        private CharacterState _state;
        private float _hpValue;
        private float _hpMax;
        private float _hpMin = 0.0f;
        [SerializeField] private GameObject[] _skillList;
        [SerializeField] private Skill[] _skills;
        [SerializeField] private float _speed;
        [SerializeField] private LayerMask _enemyMask;
        [SerializeField] private LayerMask _ballMask;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private LayerMask _wallMask;
        private float _xAxis;
        private float _zAxis;
        private Rigidbody _rigid;

        public override void OnNetworkSpawn()
        {
            if (IsOwner == false)
            {
                enabled = false;
            }

            base.OnNetworkSpawn();

            _rigid = GetComponent<Rigidbody>();

            _skills = new Skill[_skillList.Length];

            for (int i = 0; i < _skillList.Length; i++)
            {
                GameObject go = Instantiate(_skillList[i], transform);
                Skill skill = go.GetComponent<Skill>();
                skill.Init(this);
                _skills[i] = skill;
            }
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            _xAxis = Input.GetAxisRaw("Horizontal");
            _zAxis = Input.GetAxisRaw("Vertical");

            if (IsGrounded())
            {
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            }

            // Temp
            if (Input.GetMouseButtonDown(0))
            {
                _skills[0].Execute();
            }

            if (Input.GetMouseButtonDown(1))
            {
                _skills[1].Execute();
            }
        }

        private void FixedUpdate()
        {
            if (IsOwner == false)
                return;

            MovePosition(_xAxis, _zAxis);
            ChangeRotation();
        }

        private void MovePosition(float xAxis, float zAxis)
        {
            bool horizontalWallDetected = false;
            bool verticalWallDetected = false;

            if (Physics.Raycast(transform.position, new Vector3(xAxis, 0.0f, 0.0f), 0.5f, _wallMask))
            {
                horizontalWallDetected = true;
            }

            if (Physics.Raycast(transform.position, new Vector3(0.0f, 0.0f, zAxis), 0.5f, _wallMask))
            {
                verticalWallDetected = true;
            }

            if (horizontalWallDetected && verticalWallDetected)
            {
                transform.position += Vector3.zero;
            }

            else if (horizontalWallDetected)
            {
                transform.position += new Vector3(0.0f, 0.0f, zAxis) * _speed * Time.fixedDeltaTime;
            }

            else if (verticalWallDetected)
            {
                transform.position += new Vector3(xAxis, 0.0f, 0.0f) * _speed * Time.fixedDeltaTime;
            }

            else
            {
                transform.position += new Vector3(xAxis, 0.0f, zAxis) * _speed * Time.fixedDeltaTime;
            }
            
        }

        private void ChangeRotation()
        {
            transform.LookAt(transform.position + new Vector3(_xAxis, 0.0f, _zAxis));
        }

        public bool ChangeState(CharacterState state)
        {
            _state = state;

            return false;
        }

        private bool IsGrounded()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 0.15f, _groundMask);

            return cols.Length > 0;
        }

        public void DepleteHp(float amount)
        {
            _hpValue -= amount;
        }

        public void RecoverHp(float amount)
        {
            _hpValue += amount;
        }

        [ServerRpc(RequireOwnership = false)]
        public void KnockbackServerRpc(Vector3 pushDir, float pushPower)
        {
            _rigid.MovePosition(pushDir * pushPower);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawLine(transform.position, transform.position + new Vector3(_xAxis, 0.0f, 0.0f));
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0.0f, 0.0f, _zAxis));
        }
    }
}