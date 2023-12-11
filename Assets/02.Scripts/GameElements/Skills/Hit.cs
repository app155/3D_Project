using CharacterController = Project3D.Controller.CharacterController;
using Project3D.GameElements.Skill;
using UnityEngine;
using Project3D.Controller;

public class Hit : Skill
{
    private BoxCollider _col;
    private float _pushPower;
    private CharacterController owner;
    private BallController target;

    private void Start()
    {
        _col = GetComponent<BoxCollider>();
        owner = transform.root.GetComponent<CharacterController>();
        enabled = false;
    }

    public override void Execute()
    {
        if (coolTime > 0)
            return;

        Debug.Log("Hit Excuted!");
        _col.enabled = true;
    }

    private void Update()
    {
        if (coolTime > 0)
            coolTime -= Time.deltaTime;

        else if (coolTime < 0)
            coolTime = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & owner.ballMask) > 1)
        {
            other.GetComponent<IKnockback>().Knockback((other.transform.position - transform.position).normalized, _pushPower);
        }

        _col.enabled = false;
        coolTimer = coolTime;
    }
}