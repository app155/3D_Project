using Project3D.Controller;
using Project3D.GameSystem;
using Project3D.Stat;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.GameElements.Items
{
    public class RecoverItem : Item, IItem
    {
        [SerializeField] float amount;


        public override void Affect(NetworkBehaviour target)
        {
            ulong clientID = target.OwnerClientId;
            AffectServerRpc(clientID);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AffectServerRpc(ulong targetID, ServerRpcParams rpcParams = default)
        {
            IHp targetHp = InGameManager.instance.player[targetID].GetComponent<IHp>();
            Debug.Log($"target Hp Before {targetHp.hpValue}");
            targetHp.RecoverHp(amount);
            Debug.Log($"target Hp After {targetHp.hpValue}");
            AffectClientRpc(targetID);
        }

        [ClientRpc]
        public void AffectClientRpc(ulong targetID, ClientRpcParams rpcParams = default)
        {
            if (IsClient)
            {
                //gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        public override void Disappear()
        {
            DisappearServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void DisappearServerRpc(ServerRpcParams rpcParams = default)
        {
            DisappearClientRpc();
        }

        [ClientRpc]
        public void DisappearClientRpc(ClientRpcParams rpcParams = default)
        {
            if (IsClient)
            {
                Destroy(gameObject);
            }
        }
    }
}