using CharacterController = Project3D.Controller.CharacterControllers;
using Project3D.GameElements.Skill;
using UnityEngine;
using Project3D.Controller;
using Unity.Netcode;
using System;

public class Hit : Skill
{
    private float _pushPower = 10.0f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        Debug.Log("!!@#!#@!3");
    }

    public override void Init(CharacterController owner)
    {
        base.Init(owner);
        coolTime = 1.0f;
    }

    public override void Execute()
    {
        if (coolTimer > 0)
        {
            Debug.Log("[Hit] - Cooltime");
            return;
        }
        GameObject line = new GameObject("line");
        line.AddComponent<LineRenderer>();

        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
        {
            Collider[] cols = Physics.OverlapSphere(transform.position + (hit.point - transform.position).normalized, 0.5f, owner.ballMask);

            if (cols.Length > 0)
            {
                if (cols[0].TryGetComponent(out IKnockback ball))
                { 
                    lineRenderer.SetPosition(0, transform.position); // 시작점 설정
                    lineRenderer.SetPosition(1, transform.position + (hit.point - transform.position).normalized); // 끝점 설정
                    lineRenderer.startWidth = 1.0f; // 시작점 두께 설정
                    lineRenderer.endWidth = 1.0f; // 끝점 두께 설정
                    //Instantiate(_prefab, transform.position + (hit.point - transform.position).normalized, Quaternion.identity);
                    ball.KnockbackServerRpc((cols[0].transform.position - transform.position).normalized, _pushPower);
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
}