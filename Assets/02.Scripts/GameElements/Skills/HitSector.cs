using CharacterControllers = Project3D.Controller.CharacterControllers;
using Project3D.GameElements.Skill;
using UnityEngine;
using Project3D.Controller;
using Unity.Netcode;
using System;
using UnityEditor;
using System.Collections;
using DG.Tweening;
using TreeEditor;

public class HitSector : Skill
{
    private float _pushPower = 10.0f;
    public float angle;
    private SkillData _skillData;
    GameObject Range;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void Init(CharacterControllers owner)
    {
        base.Init(owner);
        _skillData = SkillDataAssets.instance.skillDatum[20];
    }

    public override void Casting()
    {
        Range = Instantiate(_skillData.Range);
        Range.transform.position =new Vector3(this.transform.position.x,Range.transform.position.y,transform.position.z);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
        {
            Range.transform.forward = (hit.point - transform.position).normalized;
        }
            
    }
    public override void Execute()
    {
        StartCoroutine("SkillRange");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 2.0f, owner.ballMask);

            if (cols.Length > 0)
            {
                if (cols[0].TryGetComponent(out IKnockback ball))
                {
                    Vector3 normal = cols[0].transform.position - transform.position;
                    Vector3 Cnormal = hit.point - transform.position;
                    Cnormal.y = 0.0f;
                    normal.y = 0.0f;
                    if (normal.magnitude <= 2.0f)
                    {
                        float dot = Vector3.Dot(normal.normalized, Cnormal.normalized);
                        float theta = Mathf.Acos(dot);
                        float degree = Mathf.Rad2Deg * theta;

                        if (degree <= angle / 2.0f)
                        {
                            ball.KnockbackServerRpc((Cnormal).normalized, _pushPower, owner.clientID);
                        }
                    }
                }
                else
                {
                    throw new Exception("[Hit] - Target Wrong");
                }

                owner.ChangeRotation(hit.point.x, hit.point.z);
            }
        }
    }


    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.color = new UnityEngine.Color(1f,0f,0f);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
        {
            Handles.DrawSolidArc(transform.position, Vector3.up, hit.point - transform.position, angle / 2, 2f);
            Handles.DrawSolidArc(transform.position, Vector3.up, hit.point - transform.position, -angle / 2, 2f);
        }
#endif
    }

    IEnumerator SkillRange()
    {
        Casting();
        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
        Destroy(Range);
    }
}