using Project3D.Controller;
using Project3D.GameSystem;
using Project3D.Stat;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;

namespace Project3D.GameElements.Items
{
    public class RecoverItem : Item
    {
        [SerializeField] float amount;

        public override void Affect(NetworkBehaviour target)
        {
            ulong clientID = target.OwnerClientId;
            // temp
            IHp targetHp = InGameManager.instance.player[clientID].GetComponent<IHp>();
            Debug.Log($"target HP Before {targetHp.HpValue}");
            AffectServerRpc(clientID);
            Debug.Log($"target Hp After {targetHp.HpValue}");
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AffectServerRpc(ulong targetID, ServerRpcParams rpcParams = default)
        {
            IHp targetHp = InGameManager.instance.player[targetID].GetComponent<IHp>();
            targetHp.RecoverHp(amount);
            gameObject.SetActive(false);
            AffectClientRpc(targetID);
        }

        [ClientRpc]
        public void AffectClientRpc(ulong targetID, ClientRpcParams rpcParams = default)
        {
            if (IsClient)
            {
                IHp targetHp = InGameManager.instance.player[targetID].GetComponent<IHp>();
                targetHp.RecoverHp(amount);
                gameObject.SetActive(false);
            }
        }
    }
}