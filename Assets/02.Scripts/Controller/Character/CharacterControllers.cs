using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Project3D.GameElements.Skill;
using System;
using Project3D.GameSystem;
using Project3D.Animations;
using UnityEngine.UI;
using Project3D.Lobbies;
using JetBrains.Annotations;
using Unity.Services.Authentication;

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
        DashAttack = 21,
        Defend = 22,
        Die,
    }
    public class CharacterControllers : NetworkBehaviour, IHp, ILv, IKnockback
    {
        public static Dictionary<ulong, CharacterControllers> spawned = new Dictionary<ulong, CharacterControllers>();
        public CharacterState state
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                if (value == CharacterState.Die)
                {
                    onDie?.Invoke();
                }
                    

                _state = value;
            }
        }

        public float speed
        {
            get => _speed;

        }

        public float hpValue
        {
            get => _hpValue.Value;
            set
            {
                if (_hpValue.Value == value)
                    return;

                _hpValue.Value = Mathf.Clamp(value, _hpMin, _hpMax);
                onHpChanged?.Invoke(value);

                if (value == _hpMax)
                    onHpMax?.Invoke();

                else if (value == _hpMin)
                    onHpMin?.Invoke();

                onDirectionChanged?.Invoke(value);
            }
        }
        public int LvValue
        {
            get => _level.Value;
            set
            {
                if (_level.Value == value)
                    return;

                _level.Value = Mathf.Clamp(value, _LvMin, _LvMax);
                onLvChanged?.Invoke((int)value);

                if (value == _LvMax)
                    onLvMax?.Invoke();

                else if (value == _LvMin)
                    onLvMin?.Invoke();

                onDirectionChanged?.Invoke(value);
            }
        }

        public float hpMax
        {
            get => _hpMax;
            set
            {
                _hpMax = value;
            }
        }
            
        public float hpMin => _hpMin;

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

        public int score
        {
            get => _score;
            set
            {
                onScore?.Invoke(clientID);

                _score = value;
            }
        }

        public int LvMax
        {
            get => _LvMax;
            set
            {
                _LvMax = value;
            }
        }

        public int LvMin => _LvMin;

        public LayerMask enemyMask => _enemyMask;
        public LayerMask ballMask => _ballMask;
        public LayerMask groundMask => _groundMask;
        public ulong clientID => OwnerClientId;


        [SerializeField]public CooltimeSlotUI slot;
        [SerializeField] CharacterData ch;
        public ExpBar expBar;
        public Team team;
        public event Action<float> onHpChanged;
        public event Action<float> onHpRecovered;
        public event Action<float> onHpDepleted;
        public event Action onHpMax;
        public event Action onHpMin;
        public event Action<float> onDirectionChanged;
        public event Action<int> onLvChanged;
        public event Action<ulong> onScore;

        public event Action onDie;
        public event Action onLvMax;
        public event Action onLvMin;

        private NetworkVariable<float> _exp;
        private NetworkVariable<int> _level;
        [SerializeField] private CharacterState _state;
        private NetworkVariable<float> _hpValue;
        private float _hpMax;
        private float _hpMin;
        private float _damage;
        private int _LvMax;
        private int _LvMin;

        [SerializeField] private int[] _skillIDs;
        public Dictionary<int, float> _skillCoolDownTimeMarks;
        [SerializeField] private float _speed;
        [SerializeField] private LayerMask _enemyMask;
        [SerializeField] private LayerMask _ballMask;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private LayerMask _wallMask;
        private float _xAxis;
        private float _zAxis;
        private bool _isStiffed;
        private bool _isWeaked;
        [SerializeField] private float _stiffTime = 0.2f;
        private float _stiffTimer;
        private Rigidbody _rigid;
        private Animator _animator;
        private Vector3 oldPosition;
        private Vector3 currentPosition;
        private double _velocity;
        [SerializeField] private int _score; //Test

        //Temp?
        [SerializeField] private GameObject _renderer;
        [SerializeField] private Canvas _hpUI;
        [SerializeField] private ParticleSystem _dieEffect;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner)
            {
                PrivateInit();
                
            }
            ProfileLoadingClientRpc();
            //temp
            ChangeState(CharacterState.Locomotion);
            _hpMax = 100;
            _hpMin = 0;
            onHpMin += () => _isWeaked = true;
            oldPosition = transform.position;

            team = GameLobbyManager.instance.lobbyPlayerDatas[(int)clientID].Team == 1 ? InGameManager.instance.blueTeam.Register(clientID) : InGameManager.instance.redTeam.Register(clientID);

            ReSetUp();
            InGameManager.instance.onStandbyState += ReSetUp;
            InGameManager.instance.onScoreState += Score;

            if (TryGetComponent(out NetworkBehaviour player))
            {
                InGameManager.instance.RegisterPlayer(clientID, player);
            }
                
            Debug.Log($"chara spawned {clientID}");

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

            spawned.Add(OwnerClientId, this);
        }

        public bool UseSkill(int skillID)
        {
            if (Time.time - _skillCoolDownTimeMarks[skillID] < SkillDataAssets.instance[skillID].coolDownTime)
            {
                Debug.Log("CoolT");
                return false;
            }
            _skillCoolDownTimeMarks[skillID] = Time.time;
            Skill skill = Instantiate(SkillDataAssets.instance[skillID].skill, transform);
            skill.Init(this);
            skill.Execute();
            //expBar.IncreaseExp(40);
            ChangeState((CharacterState)skillID);
            return true;
        }

        private void Awake()
        {
            
            _exp = new NetworkVariable<float>(0.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            _level = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            _hpValue = new NetworkVariable<float>(80.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            _hpValue.OnValueChanged += (prev, current) =>
            {
                onHpChanged?.Invoke(current);
                if (prev < current)
                {
                    onHpRecovered?.Invoke(current - prev);
                }
                else if (prev > current)
                {
                    onHpDepleted?.Invoke(prev - current);
                }
                if (current == _hpMax)
                {
                    onHpMax?.Invoke();
                }
                else if (current == _hpMin)
                {
                    onHpMin?.Invoke();
                }
            };

            _rigid = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
            AnimBehaviour[] animBehaviours = _animator.GetBehaviours<AnimBehaviour>();
            for (int i = 0; i < animBehaviours.Length; i++)
            {
                animBehaviours[i].Init(this);
            }

            _skillCoolDownTimeMarks = new Dictionary<int, float>();
            foreach (var skillID in _skillIDs)
            {
                _skillCoolDownTimeMarks.Add(skillID, 0.0f);
            }
        }
        [ClientRpc]
        public void ProfileLoadingClientRpc()
        {
            if (!IsOwner)
                return;

            slot = Instantiate(slot);
            slot.slot1.data = SkillDataAssets.instance.skillDatum[_skillIDs[0]];
            slot.slot2.data = SkillDataAssets.instance.skillDatum[_skillIDs[1]];
            Image profileImage = slot.profile.GetComponent<Image>();
            profileImage.material = ch.profile.material;
            Image Skill1 = slot.slot1._icon.GetComponent<Image>();
            Image Skill2 = slot.slot2._icon.GetComponent<Image>();
            Skill1.material = SkillDataAssets.instance.skillDatum[_skillIDs[0]].icon.material;
            Skill2.material = SkillDataAssets.instance.skillDatum[_skillIDs[1]].icon.material;
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
                    //_xAxis = Input.GetAxisRaw("Horizontal");
                    //_zAxis = Input.GetAxisRaw("Vertical");
                }

                else
                {
                    if (_stiffTimer < _stiffTime)
                    {
                        _stiffTimer += Time.deltaTime;
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

            MovePosition(xAxis, zAxis);
            GetVelocity();

            if (_isStiffed == false)
            {
                ChangeRotation(xAxis, zAxis);
            }
        }

        public void PrivateInit()
        {
            // temp
            onDie += () =>
            {
                StartCoroutine(C_OnDie());
            };

            // Key Mapping
            InputSystem.instance.maps["Player"].RegisterAxisAction("Horizontal", (value) =>
            {
                if (_isStiffed == false)
                {
                    _xAxis = value;
                }
            });

            InputSystem.instance.maps["Player"].RegisterAxisAction("Vertical", (value) => 
            {
                if (_isStiffed == false)
                {
                    _zAxis = value;
                }
            });

            // ����
            InputSystem.instance.maps["Player"].RegisterMouseDownAction(0, () =>
            {
                if (UseSkill(_skillIDs[0]))
                {
                    slot.cooltimeCheckTest(slot.slot1);
                }
                
            });

            // 1
            InputSystem.instance.maps["Player"].RegisterMouseDownAction(1, () =>
            {
                if (UseSkill(_skillIDs[1]))
                {
                    slot.cooltimeCheckTest(slot.slot2);
                }
            });

            // 2��°
            InputSystem.instance.maps["Player"].RegisterKeyDownAction(KeyCode.LeftShift, () =>
            {
                UseSkill(_skillIDs[2]);
            });

            // 3
            InputSystem.instance.maps["Player"].RegisterKeyDownAction(KeyCode.Space, () =>
            {
                UseSkill(_skillIDs[3]);
            });
        }
        
        public virtual void ReSetUp()
        {
            RecoverHp(hpMax);
            xAxis = 0.0f;
            zAxis = 0.0f;
            Spawn();
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
                Vector3 moveDir = new Vector3(xAxis, 0.0f, zAxis);

                if (_isStiffed)
                    transform.position += moveDir * (Convert.ToInt32(_isWeaked) + 1) * _speed * Time.fixedDeltaTime;

                else if (state == CharacterState.Locomotion)
                    transform.position += moveDir.normalized * _speed * Time.fixedDeltaTime;

                else
                    transform.position += moveDir * _speed * Time.fixedDeltaTime;

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

        public void ChangeRotation(float xAxis, float zAxis)
        {
            _renderer.transform.LookAt(transform.position + new Vector3(xAxis, 0.0f, zAxis));
        }

        private void GetVelocity()
        {
            currentPosition = _rigid.position;
            Vector3 dis = (currentPosition - oldPosition);
            var distance = Math.Sqrt(Math.Pow(dis.x, 2) + Math.Pow(dis.y, 2) + Math.Pow(dis.z, 2));
            _velocity = distance / Time.fixedDeltaTime;
            oldPosition = currentPosition;
            _animator.SetFloat("Velocity", Convert.ToSingle(_velocity));
        }

        public bool ChangeState(CharacterState newState)
        {
            if (state == newState)
                return false;

            ChangeStateServerRpc(newState);
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
            _animator.SetInteger("state", (int)newState);
            _animator.SetBool("isDirty", true);
            
            state = newState;
        }

        private bool IsGrounded()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 0.15f, _groundMask);

            return cols.Length > 0;
        }

        public void DepleteHp(float amount)
        {
            DepleteHpServerRpc(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DepleteHpServerRpc(float amount)
        {
            hpValue -= amount;
            onHpDepleted?.Invoke(amount);

            //DepleteHpClientRpc(amount);
        }

        [ClientRpc]
        public void DepleteHpClientRpc(float amount)
        {
            if (IsServer)
                return;

            hpValue -= amount;
            onHpDepleted?.Invoke(amount);
        }

        public void RecoverHp(float amount)
        {
            RecoverHpServerRpc(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RecoverHpServerRpc(float amount)
        {
            hpValue += amount;
            onHpRecovered?.Invoke(amount);

            //DepleteHpClientRpc(amount);
        }

        public void LvUp(int amount)
        {
            LvValue += amount;
            onLvChanged?.Invoke(amount);
        }

        [ServerRpc(RequireOwnership = false)]
        public void KnockbackServerRpc(Vector3 pushDir, float pushPower, ulong clientID, ServerRpcParams rpcParams = default)
        {
            _isStiffed = true;
            xAxis = pushDir.x * pushPower;
            zAxis = pushDir.z * pushPower;
            InGameManager.instance.player[clientID].GetComponent<CharacterControllers>().expBar.IncreaseExpServerRpc((int)Formulas.CalcExp(1f, 1));

            KnockbackClientRpc(pushDir, pushPower, clientID);
        }

        [ClientRpc]
        public void KnockbackClientRpc(Vector3 pushDir, float pushPower, ulong clientID, ClientRpcParams rpcParams = default)
        {
            _isStiffed = true;
            xAxis = pushDir.x * pushPower;
            zAxis = pushDir.z * pushPower;
        }

        private IEnumerator C_OnDie()
        {
            Debug.Log("ondie routine start");

            DisappearServerRpc();

            yield return new WaitForSeconds(3.0f);

            RespawnServerRpc();
        }

        public void Disappear()
        {
            DisappearServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void DisappearServerRpc()
        {
            _renderer.SetActive(false);
            _hpUI.enabled = false;
            _dieEffect.gameObject.SetActive(true);
            _dieEffect.Play();

            DisappearClientRpc();
        }

        [ClientRpc]
        public void DisappearClientRpc()
        {
            _renderer.SetActive(false);
            _hpUI.enabled = false;
            _dieEffect.gameObject.SetActive(true);
            xAxis = 0.0f;
            zAxis = 0.0f;
            _dieEffect.Play();
        }

        public void Spawn()
        {
            SpawnServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnServerRpc()
        {
            List<ulong> playerInTeam = new List<ulong>();
            playerInTeam = team.GetPlayersInTeam();
            int playercount = 0;

            for (int i = 0; i < playerInTeam.Count; i++)
            {
                if (playerInTeam[i] == clientID)
                {
                    playercount = i;
                }
            }
            if (team.id == 0)
            {
                transform.position = InGameManager.instance.spawnPoints[playercount * 2].position;

            }
            else if (team.id == 1)
            {
                transform.position = InGameManager.instance.spawnPoints[1 + playercount * 2].position;
            }

            SpawnClientRpc();
        }

        [ClientRpc]
        public void SpawnClientRpc()
        {
            List<ulong> playerInTeam = new List<ulong>();
            playerInTeam = team.GetPlayersInTeam();
            int playercount = 0;

            for (int i = 0; i < playerInTeam.Count; i++)
            {
                if (playerInTeam[i] == clientID)
                {
                    playercount = i;
                }
            }
            if (team.id == 0)
            {
                transform.position = InGameManager.instance.spawnPoints[playercount * 2].position;

            }
            else if (team.id == 1)
            {
                transform.position = InGameManager.instance.spawnPoints[1 + playercount * 2].position;
            }
        }

        public void Respawn()
        {
            RecoverHp(hpMax);
            RespawnServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RespawnServerRpc()
        {
            transform.position = InGameManager.instance.spawnPoints[team.id + 6].position;
            ChangeState(CharacterState.Locomotion);
            _renderer.SetActive(true);
            gameObject.SetActive(true);
            _hpUI.enabled = true;
            _dieEffect.gameObject.SetActive(false);
            RecoverHp(hpMax);

            AnimBehaviour[] animBehaviours = _animator.GetBehaviours<AnimBehaviour>();
            for (int i = 0; i < animBehaviours.Length; i++)
            {
                animBehaviours[i].Init(this);
            }

            RespawnClientRpc();
        }

        [ClientRpc]
        public void RespawnClientRpc()
        {
            transform.position = InGameManager.instance.spawnPoints[team.id + 6].position;
            ChangeState(CharacterState.Locomotion);
            _renderer.SetActive(true);
            gameObject.SetActive(true);
            _hpUI.enabled = true;
            _dieEffect.gameObject.SetActive(false);

            AnimBehaviour[] animBehaviours = _animator.GetBehaviours<AnimBehaviour>();
            for (int i = 0; i < animBehaviours.Length; i++)
            {
                animBehaviours[i].Init(this);
            }
        }

        public void Score()
        {
            ScoreServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void ScoreServerRpc()
        {

            ScoreClientRpc();
        }

        [ClientRpc]
        public void ScoreClientRpc()
        {
            if (InGameManager.instance.player.TryGetValue(InGameManager.instance.scorerID, out NetworkBehaviour player))
            {
                if (clientID == player.OwnerClientId)
                {
                    score++;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
        }
    }
}