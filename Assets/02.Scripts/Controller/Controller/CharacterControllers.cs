using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Project3D.GameElements.Skill;
using System;
using System.Security.Cryptography;
using UnityEngine.UIElements;
using Project3D.GameSystem;
using Project3D.Animations;

namespace Project3D.Controller
{
    public enum CharacterState
    {
        None,
        Locomotion,
        Respawned,
        Hit,
        Ceremony,
        Attack = 20,
        Die,
    }
    public class CharacterControllers : NetworkBehaviour, IHp, IKnockback
    {
        static Dictionary<ulong, CharacterControllers> _spawned = new Dictionary<ulong, CharacterControllers>();

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
            set
            {
                _hpMax = value;
            }
        }
            
        public float HpMin => _hpMin;

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

        public ulong clientID => OwnerClientId;
        public int Lv { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Team team;
        public event Action<float> onHpChanged;
        public event Action<float> onHpRecovered;
        public event Action<float> onHpDepleted;
        public event Action onHpMax;
        public event Action onHpMin;
        public event Action<float> onDirectionChanged;
        public event Action<int> onLvChanged;

        NetworkVariable<float> _exp;
        NetworkVariable<int> _level;
        [SerializeField] private CharacterState _state;
        private float _hpValue;
        private float _hpMax;
        private float _hpMin;
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
        private Animator _animator;
        int getdamaged;
        private Vector3 oldPosition;
        private Vector3 currentPosition;
        private double _velocity;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                // temp
                TestUI_Hp.testHp.chara = this;
            }

            ChangeState(CharacterState.Locomotion);
            _hpMax = 100;
            _hpMin = 0;
            _hpValue = 80; // temp
            oldPosition = transform.position;

            // temp
            team = InGameManager.instance.blueTeam;

            _skills = new Skill[_skillList.Length];

            Debug.Log($"chara spawned : {OwnerClientId}");

            for (int i = 0; i < _skillList.Length; i++)
            {
                GameObject go = Instantiate(_skillList[i], transform);
                Skill skill = go.GetComponent<Skill>();
                skill.Init(this);
                _skills[i] = skill;
            }

            if (TryGetComponent(out NetworkBehaviour player))
            {
                InGameManager.instance.RegisterPlayer(player.OwnerClientId, player);
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
            _animator = GetComponent<Animator>();
            _rigid = GetComponent<Rigidbody>();
            AnimBehaviour[] animBehaviours = _animator.GetBehaviours<AnimBehaviour>();
            for (int i = 0; i < animBehaviours.Length; i++)
            {
                animBehaviours[i].Init(this);
            }
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            if (IsGrounded())
            {
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);

                if (_isStiffed == false)
                {
                    _xAxis = Input.GetAxisRaw("Horizontal");
                    _zAxis = Input.GetAxisRaw("Vertical");

                    // Temp SkillACtion Input
                    if (Input.GetMouseButtonDown(0))
                    {
                        ChangeState(CharacterState.Attack);
                        _skills[0].Execute();
                    }

                    if (Input.GetMouseButton(0))
                    {
                        _skills[1].Casting();
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        _skills[1].Execute();
                    }

                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        _skills[1].Execute();
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        _skills[2].Execute();
                    }
                }

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

            else
            {
                ChangeState(CharacterState.Die);
            }
        }

        private void FixedUpdate()
        {
            if (IsOwner == false)
                return;

            if (state == CharacterState.Die)
                return;

            MovePosition(_xAxis, _zAxis);
            GetVelocity();

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

            if (InGameManager.instance.gameState != GameState.Playing &&
                InGameManager.instance.gameState != GameState.Score)
                return;

            bool horizontalWallDetected = false;
            bool verticalWallDetected = false;

            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, new Vector3(xAxis, 0.0f, 0.0f), 0.5f, _wallMask))
            {
                horizontalWallDetected = true;
            }

            if (Physics.Raycast(transform.position + Vector3.up * 0.2f, new Vector3(0.0f, 0.0f, zAxis), 0.5f, _wallMask))
            {
                verticalWallDetected = true;
            }

            if ((horizontalWallDetected == false && verticalWallDetected == false) || _isStiffed)
            {
                _rigid.position += new Vector3(xAxis, 0.0f, zAxis) * _speed * Time.fixedDeltaTime;
            }

            else if (horizontalWallDetected && verticalWallDetected)
            {
                _rigid.position += Vector3.zero;
            }

            else if (horizontalWallDetected)
            {
                _rigid.position += new Vector3(0.0f, 0.0f, zAxis) * _speed * Time.fixedDeltaTime;
            }

            else if (verticalWallDetected)
            {
                _rigid.position += new Vector3(xAxis, 0.0f, 0.0f) * _speed * Time.fixedDeltaTime;
            }
        }

        private void ChangeRotation()
        {
            transform.LookAt(transform.position + new Vector3(_xAxis, 0.0f, _zAxis));
        }

        private void GetVelocity()
        {
            currentPosition = transform.position;
            Vector3 dis = (currentPosition - oldPosition);
            var distance = Math.Sqrt(Math.Pow(dis.x, 2) + Math.Pow(dis.y, 2) + Math.Pow(dis.z, 2));
            _velocity = distance / Time.deltaTime;
            oldPosition = currentPosition;
            _animator.SetFloat("Velocity", Convert.ToSingle(_velocity));
        }

        public bool ChangeState(CharacterState newState)
        {
            if (state == newState)
                return false;

            _animator.SetInteger("state", (int)newState);
            _animator.SetBool("isDirty", true);
            _state = newState;
            return true;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeStateServerRpc(CharacterState newState, ServerRpcParams rpcParams = default)
        {
            ChangeStateClientRpc(newState);
        }

        [ClientRpc]
        public void ChangeStateClientRpc(CharacterState newState, ClientRpcParams rpcParams = default)
        {
            
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
        public void KnockbackServerRpc(Vector3 pushDir, float pushPower, ulong clientID, ServerRpcParams rpcParams = default)
        {
            _isStiffed = true;
            xAxis = pushDir.x * pushPower;
            zAxis = pushDir.z * pushPower;

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

            Gizmos.DrawLine(transform.position + Vector3.up * 0.2f, transform.position + new Vector3(_xAxis, 0.0f, 0.0f));
            Gizmos.DrawLine(transform.position + Vector3.up * 0.2f, transform.position + new Vector3(0.0f, 0.0f, _zAxis));
        }

        public void Attack(float amount)
        {

        }
    }
}