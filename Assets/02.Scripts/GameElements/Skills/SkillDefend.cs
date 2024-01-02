using Project3D.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Project3D.GameElements.Skill
{
    public class SkillDefend : Skill
    {
        private float _pushPower = 1f;
        private SkillData _skillData;
        GameObject Range;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public override void Init(CharacterControllers owner)
        {
            base.Init(owner);
            _skillData = SkillDataAssets.instance.skillDatum[22];
        }

        public override void Casting()
        {
            Range = Instantiate(_skillData.Range);
            Range.transform.position = transform.position;
            
        }

        public override bool Execute()
        {
            StartCoroutine("SkillRange");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
            {
                Collider[] cols = Physics.OverlapSphere(transform.position, 3.0f, owner.ballMask);

                if (cols.Length > 0)
                {
                    if (cols[0].TryGetComponent(out IKnockback ball))
                    {
                        Debug.Log(ball);
                        ball.KnockbackServerRpc((hit.point - cols[0].transform.position).normalized, _pushPower, owner.clientID);
                        owner.ChangeRotation(hit.point.x, hit.point.z);
                        return true;
                    }
                    else
                    {
                        throw new Exception("[Hit] - Target Wrong");
                    }
                    
                }
            }
            owner.ChangeRotation(hit.point.x, hit.point.z);
            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position,3.0f);
        }
        IEnumerator SkillRange()
        {
            Casting();
            yield return new WaitForSeconds(1.0f);
            Destroy(Range);
            Destroy(this.gameObject);
        }
    }
}

