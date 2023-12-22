using CharacterControllers = Project3D.Controller.CharacterControllers;
using Project3D.GameElements.Skill;
using UnityEngine;
using Project3D.Controller;
using Unity.Netcode;
using System;
using UnityEditor;

public class HitSector : Skill
{
    private float _pushPower = 10.0f;
    public float angle;
    public GameObject Range;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public override void Init(CharacterControllers owner)
    {
        base.Init(owner);
        Range.SetActive(false);
        coolTime = 1.0f;
    }

    public override void Casting()
    {
        Range.SetActive(true);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
        {
            Range.transform.forward = (hit.point-transform.position).normalized;
        }
            
    }
    public override void Execute()
    {
        if (coolTimer > 0)
        {
            Debug.Log("[Hit] - Cooltime");
            return;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 2.0f, owner.ballMask);

            if (cols.Length > 0)
            {
                Range.SetActive(false);
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
                            ball.KnockbackServerRpc((Cnormal).normalized, _pushPower, OwnerClientId);
                        }
                    }
                }
                else
                {
                    throw new Exception("[Hit] - Target Wrong");
                }

                Debug.Log("Hit Ball");
            }
        }

        coolTimer = coolTime;
    }

    private void Update()
    {
        if (coolTimer > 0)
            coolTimer -= Time.deltaTime;

        else if (coolTimer < 0)
            coolTimer = 0;
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
        // DrawSolidArc(������, ��ֺ���(��������), �׷��� ���� ����, ����, ������)
#endif
    }
    /*private void OnDrawGizmos()
    {
        DrawCube();
        DrawRay();
    }
   

    void DrawCube()
    {
        Gizmos.color = Color.red;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
        {
            Gizmos.DrawWireSphere(transform.position + (hit.point - transform.position).normalized, 0.5f);
        }
    }

    void DrawRay()
    {
        Gizmos.color = Color.yellow;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Gizmos.DrawRay(ray.origin, ray.direction * 30.0f);
    }
     */
}