using Project3D.Controller;
using Project3D.Stat;
using Unity.Netcode;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;

namespace Project3D.GameElements.Items
{
    public class RecoverItem : Item, IItem
    {
        [SerializeField] float amount;

        public override void Affect(Transform target)
        {
            if (target.TryGetComponent(out IHp targetHp))
            {
                Debug.Log($"target HP Before {targetHp.HpValue}");
                targetHp.RecoverHp(amount);
                Debug.Log($"target Hp After {targetHp.HpValue}");
                AffectServerRpc();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AffectServerRpc(ServerRpcParams rpcParams = default)
        {
            //targetHp.DepleteHp(amount);
            //gameObject.SetActive(false);
            //AffectClientRpc(targetHp);
        }

        [ClientRpc]
        public void AffectClientRpc(ClientRpcParams rpcParams = default)
        {
            //targetHp.DepleteHp(amount);
            //gameObject.SetActive(false);
        }
    }
}