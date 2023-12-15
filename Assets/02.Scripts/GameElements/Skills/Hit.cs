using CharacterController = Project3D.Controller.CharacterControllers;
using Project3D.GameElements.Skill;
using UnityEngine;
using Project3D.Controller;
using Unity.Netcode;
using System;

public class Hit : Skill
{
    private float _pushPower = 10.0f;

    public override void Execute()
    {
        if (coolTime > 0)
            return;

        Collider[] cols = Physics.OverlapBox(transform.position + Vector3.forward, Vector3.one, Quaternion.identity, owner.ballMask);

        if (cols.Length > 0)
        {
            if (cols[0].TryGetComponent(out IKnockback ball))
            {
                ball.Knockback((cols[0].transform.position - transform.position).normalized, _pushPower);
            }

            else
            {
                throw new Exception("[Hit] - Target Wrong");
            }

            Debug.Log("Hit Ball");
        }
    }

    private void Update()
    {
        if (coolTime > 0)
            coolTime -= Time.deltaTime;

        else if (coolTime < 0)
            coolTime = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 tempMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mousePos = new Vector3(tempMousePos.x, 0.0f, tempMousePos.z);

        Gizmos.DrawWireCube(transform.position + (mousePos - transform.position).normalized + Vector3.down * 0.3f, Vector3.one);
    }
}