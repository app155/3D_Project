using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Project3D.GameElements.Skill;
using System;
using static UnityEditor.PlayerSettings;
using System.Security.Cryptography;
using UnityEngine.UIElements;

namespace Project3D.Controller
{
    public class CharacterControllers : NetworkBehaviour, IHp, IKnockback
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
                onHpChanged?.Invoke(value);

                if (value == _hpMax)
                    onHpMax?.Invoke();
                else if (value == _hpMin)
                    onHpMin?.Invoke();
                onDirectionChanged?.Invoke(value);
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

        public int Lv { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event Action<float> onHpChanged;
        public event Action<float> onHpRecovered;
        public event Action<float> onHpDepleted;
        public event Action onHpMax;
        public event Action onHpMin;
        public event Action<float> onDirectionChanged;
        public event Action<int> onLvChanged;

        NetworkVariable<float> _exp;
        NetworkVariable<int> _level;

        private CharacterState _state;
        private float _hpValue;
        private float _hpMax;
        private float _hpMin = 0.0f;
        private float _damage;
        [SerializeField] private GameObject[] _skillList;
        [SerializeField] private Skill[] _skills;
        [SerializeField] private float _speed;
        [SerializeField] private LayerMask _enemyMask;
        [SerializeField] private LayerMask _ballMask;
        [SerializeField] private LayerMask _groundMask;
        private Rigidbody _rigid;
        int getdamaged;
        


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

            if (IsServer)
            {
                _exp.OnValueChanged += (prev, current) =>
                {
                    if (current >= 100.0f)
                    {
                        _exp.Value = current - 100.0f;
                        _level.Value++;
                        _hpMax += 500.0f;
                        _damage += 15.0f;
                    }
                };
            }

        }

        private void Awake()
        {
            _exp = new NetworkVariable<float>(0.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            _level = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
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

        public virtual void SetUp()
        {
            _hpValue = _hpMax;
            
        }
        private void MovePosition()
        {
            transform.position += new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")) * Time.fixedDeltaTime;
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
            onHpDepleted?.Invoke(amount);

        }

        public void RecoverHp(float amount)
        {
            _hpValue += amount;
            onHpRecovered?.Invoke(amount);
        }

        public void Knockback(Vector3 pushDir, float pushPower)
        {
            _rigid.MovePosition(pushDir * pushPower);
        }

        public void Attack(float amount)
        {

        }
    }
}