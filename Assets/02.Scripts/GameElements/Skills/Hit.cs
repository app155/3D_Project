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
    }

    public override void Init(CharacterController owner)
    {
        base.Init(owner);
    }

    public override void Execute()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, owner.groundMask))
        {
            Collider[] cols = Physics.OverlapSphere(transform.position + (hit.point - transform.position).normalized * 0.5f, 0.5f, owner.ballMask);

            if (cols.Length > 0)
            {
                if (cols[0].TryGetComponent(out IKnockback ball))
                {  
                    ball.KnockbackServerRpc((hit.point - cols[0].transform.position).normalized, _pushPower, owner.clientID);
                }
                else
                {
                    throw new Exception("[Hit] - Target Wrong");
                }

                Debug.Log("Hit Ball");
            }

            owner.transform.LookAt(hit.point);
        }
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

    public override void Casting()
    {
    }
}