using Project3D.Controller;
using Unity.Netcode;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterController;

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
            gameObject.SetActive(false);
            AffectClientRpc();
        }

        [ClientRpc]
        public void AffectClientRpc(ClientRpcParams rpcParams = default)
        {
            gameObject.SetActive(false);
        }
    }
}