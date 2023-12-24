using CharacterControllers = Project3D.Controller.CharacterControllers;
using Project3D.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project3D.GameElements.Skill
{
    [RequireComponent(typeof(BoxCollider))]
    public class DashAttack : Skill, IAttack
    {
        private BoxCollider _col;
        private float _ballPushPower = 10.0f;
        private float _characterPushPower = 5.0f;
        private float _atkGain;
        private float _dashSpeed = 4.0f;
        private bool _isExecuting;
        private Vector3 _executeDir;
        private HashSet<GameObject> _hits;
        private SkillData _skillData;

        public override void Init(CharacterControllers owner)
        {
            base.Init(owner);
            _col = GetComponent<BoxCollider>();
            _col.enabled = false;
            _col.size = Vector3.one;
            _col.isTrigger = true;
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
                }
            }

            else if ((1 << other.gameObject.layer & owner.enemyMask) > 0 && other.GetComponent<CharacterControllers>().team.id != owner.team.id)
            {
                if (_hits.Contains(other.gameObject) == false)
                {
                    if (other.TryGetComponent(out IHp target))
                    {
                        target.KnockbackServerRpc((other.transform.position - transform.position).normalized, _characterPushPower, owner.clientID);
                        Attack(target);
                        _hits.Add(other.gameObject);
                    }
                }
            }
        }

        public void Attack(IHp target)
        {
            target.DepleteHp(10.0f);
        }

        public override void Execute()
        {
            _hits.Clear();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
            {
                _executeDir = (hit.point - transform.position).normalized;
                _isExecuting = true;
                owner.transform.LookAt(hit.point);
                StartCoroutine(C_Execute(_executeDir));
            }
        }

        IEnumerator C_Execute(Vector3 direction)
        {
            _col.enabled = true;

            Debug.Log("dash coroutine start");

            while (castTimer > 0)
            {
                castTimer -= Time.deltaTime;
                owner.xAxis = direction.x * _dashSpeed;
                owner.zAxis = direction.z * _dashSpeed;
                yield return null;
            }

            castTimer = castTime;
            _col.enabled = false;
            Debug.Log("dash coroutine end");
            //owner.ChangeState(CharacterState.Locomotion);
            //Destroy(gameObject);
        }

        public override void Casting()
        {

        }
    }
}