using CharacterControllers = Project3D.Controller.CharacterControllers;
using Project3D.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Project3D.GameSystem;

namespace Project3D.GameElements.Skill
{
    [RequireComponent(typeof(BoxCollider))]
    public class DashAttack : Skill, IAttack
    {
        private BoxCollider _col;
        [SerializeField] private float _ballPushPower = 10.0f;
        [SerializeField] private float _characterPushPower = 4.0f;
        private float _atkGain;
        private float _dashSpeed = 5.0f;
        private bool _isExecuting;
        private Vector3 _executeDir;
        private HashSet<GameObject> _hits;
        private SkillData _skillData;

        public override void Init(CharacterControllers owner)
        {
            base.Init(owner);
            _col = GetComponent<BoxCollider>();
            _col.size = Vector3.one;
            castTime = 0.3f;
            _isExecuting = false;

            _hits = new HashSet<GameObject>();
            _skillData = SkillDataAssets.instance.skillDatum[21];
        }


        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & owner.ballMask) > 0)
            {
                if (_hits.Contains(other.gameObject) == false)
                {
                    other.GetComponent<IKnockback>().KnockbackServerRpc((other.transform.position - transform.position).normalized, _ballPushPower, owner.clientID);
                    _hits.Add(other.gameObject);
                    owner.expBar.IncreaseExpServerRpc((int)Formulas.CalcExp(1f, 1));
                }
            }

            else if ((1 << other.gameObject.layer & owner.enemyMask) > 0)
            {
                if (other.TryGetComponent(out CharacterControllers chara) && chara.team.id != owner.team.id)
                {
                    if (_hits.Contains(other.gameObject) == false)
                    {
                        if (other.TryGetComponent(out IHp target))
                        {
                            Debug.Log($"owner : {owner.clientID} knockback {chara.clientID}");
                            target.KnockbackServerRpc((other.transform.position - transform.position).normalized, _characterPushPower, owner.clientID);
                            Attack(chara.clientID);
                            _hits.Add(other.gameObject);
                            owner.expBar.IncreaseExpServerRpc((int)Formulas.CalcExp(1f, 1));
                        }
                    }
                }
            }
        }

        public void Attack(ulong targetID)
        {
            IHp target = InGameManager.instance.player[targetID].GetComponent<IHp>();
            Debug.Log($"target hp deplete before : {target.hpValue}");
            target.DepleteHp(40.0f); // temp 
            Debug.Log($"target hp deplete after : {target.hpValue}");
        }

        public override void Execute()
        {
            _hits.Clear();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
            {
                _executeDir = (hit.point - transform.position).normalized;
                _isExecuting = true;
                StartCoroutine(C_Execute(_executeDir));
            }

            owner.ChangeRotation(hit.point.x, hit.point.z);
        }

        IEnumerator C_Execute(Vector3 direction)
        {
            Debug.Log("dash coroutine start");

            float startTime = Time.time;

            while (Time.time - startTime < castTime)
            {
                Debug.Log(Time.time - startTime);

                owner.xAxis = direction.x * _dashSpeed;
                owner.zAxis = direction.z * _dashSpeed;

                yield return null;
            }

            castTimer = castTime;
            Debug.Log("dash coroutine end");
            owner.ChangeState(CharacterState.Locomotion);
            Destroy(gameObject);
        }

        public override void Casting()
        {

        }
    }
}