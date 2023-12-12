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

        public LayerMask enemyMask { get => _enemyMask; }
        public LayerMask ballMask => _ballMask;

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
            if (IsGrounded())
            {
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            }

            // Temp
            if (Input.GetMouseButtonDown(0))
            {
                _skills[0].GetComponent<Skill>().Execute();
            }

            if (Input.GetMouseButtonDown(1))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Knockback(pos.normalized, Vector3.Distance(pos, transform.position));
            }
        }

        private void FixedUpdate()
        {
            if (IsOwner == false)
                return;

            MovePosition();
            ChangeRotation();            
        }

        private void MovePosition()
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")) * _speed * Time.fixedDeltaTime;
        }

        private void ChangeRotation()
        {
            transform.LookAt(transform.position + new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")));
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

        public void Knockback(Vector3 pushDir, float pushPower)
        {
            _rigid.MovePosition(pushDir * pushPower);
        }
    }
}