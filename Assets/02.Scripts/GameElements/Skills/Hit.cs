using CharacterController = Project3D.Controller.CharacterController;
using Project3D.GameElements.Skill;
using UnityEngine;
using Project3D.Controller;

public class Hit : Skill
{
    private float _pushPower;
    [SerializeField] private CharacterController _owner;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _owner = transform.root.GetComponent<CharacterController>();
    }

    private void Awake()
    {
        _owner = transform.root.GetComponent<CharacterController>();
    }

    public override void Execute()
    {
        if (coolTime > 0)
            return;

        Collider[] cols = Physics.OverlapBox(gameObject.transform.position + Vector3.forward, Vector3.one, Quaternion.identity, _owner.ballMask);

        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                Debug.Log(col.gameObject.name);
            }

            cols[0].GetComponent<IKnockback>().Knockback(Vector3.forward, 10.0f);
        }

        Debug.Log("Hit Ball");
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

        Gizmos.DrawWireCube(transform.position + Vector3.forward + Vector3.down * 0.3f, Vector3.one);
    }
}