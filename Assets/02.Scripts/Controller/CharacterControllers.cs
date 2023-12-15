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
        static Dictionary<ulong, CharacterController> _spawned = new Dictionary<ulong, CharacterController>();

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

        public CharacterState state
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                _state = value;
            }
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

        public int clientID;
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
        [SerializeField] private float _hpValue;
        private float _hpMax;
        private float _hpMin = 0.0f;
        private float _damage;
        [SerializeField] private GameObject[] _skillList;
        [SerializeField] private Skill[] _skills;
        [SerializeField] private float _speed;
        [SerializeField] private LayerMask _enemyMask;
        [SerializeField] private LayerMask _ballMask;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private LayerMask _wallMask;
        private float _xAxis;
        private float _zAxis;
        private bool _isStiffed;
        [SerializeField] private float _stiffTime = 0.2f;
        private float _stiffTimer;
        private Rigidbody _rigid;
        int getdamaged;
        


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _rigid = GetComponent<Rigidbody>();
            _state = CharacterState.Locomotive;
            _hpMax = 100;
            _hpMin = 0;
            _hpValue = 80; // temp

            _skills = new Skill[_skillList.Length];

            for (int i = 0; i < _skillList.Length; i++)
            {
                GameObject go = Instantiate(_skillList[i], transform);
                Skill skill = go.GetComponent<Skill>();
                skill.Init(this);
                _skills[i] = skill;
            }

            // Temp
            if (IsOwner == false)
                return;
            TestUI_Hp.testHp.chara = this;
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
            if (!IsOwner)
                return;

            if (IsGrounded())
            {
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            }

            // ���� �ƴ� ��
            if (_isStiffed == false)
            {
                _xAxis = Input.GetAxisRaw("Horizontal");
                _zAxis = Input.GetAxisRaw("Vertical");

                // Temp SkillACtion Input
                if (Input.GetMouseButtonDown(0))
                {
                    _skills[0].Execute();
                }

                if (Input.GetMouseButtonDown(1))
                {
                    _skills[1].Execute();
                }
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _skills[1].Execute();
            }

            // ���� ��
            else
            {
                if (_stiffTimer < _stiffTime)
                {
                    _stiffTimer += Time.deltaTime;
                }

                else
                {
                    _stiffTimer = 0;
                    _isStiffed = false;
                }
            }
        }

        private void FixedUpdate()
        {
            if (IsOwner == false)
                return;

            MovePosition(_xAxis, _zAxis);

            if (_isStiffed == false)
            {
                ChangeRotation();
            }

        }

        
        public virtual void SetUp()
        {
            _hpValue = _hpMax;
            
        }
        private void MovePosition(float xAxis, float zAxis)
        {
            if (IsOwner == false)
                return;

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

            if ((horizontalWallDetected == false && verticalWallDetected == false) || _isStiffed)
            {
                transform.position += new Vector3(xAxis, 0.0f, zAxis) * _speed * Time.fixedDeltaTime;
            }

            else if (horizontalWallDetected && verticalWallDetected)
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
        }

        private void ChangeRotation()
        {
            transform.LookAt(transform.position + new Vector3(_xAxis, 0.0f, _zAxis));
        }

        public bool ChangeState(CharacterState state)
        {
            _state = state;
            return true;
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

        [ServerRpc(RequireOwnership = false)]
        public void KnockbackServerRpc(Vector3 pushDir, float pushPower, ServerRpcParams rpcParams = default)
        {
            _isStiffed = true;
            xAxis = pushDir.x * pushPower;
            zAxis = pushDir.z * pushPower;

            ulong clientID = rpcParams.Receive.SenderClientId;
            KnockbackClientRpc(pushDir, pushPower, clientID);
        }

        [ClientRpc]
        public void KnockbackClientRpc(Vector3 pushDir, float pushPower, ulong clientID, ClientRpcParams rpcParams = default)
        {
            _isStiffed = true;
            xAxis = pushDir.x * pushPower;
            zAxis = pushDir.z * pushPower;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawLine(transform.position, transform.position + new Vector3(_xAxis, 0.0f, 0.0f));
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0.0f, 0.0f, _zAxis));
        }

        public void Attack(float amount)
        {

        }
    }
}