using Project3D.Controller;
using Project3D.GameSystem;
using Project3D.Stat;
using Unity.Netcode;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;

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
            AffectClientRpc(targetID);
        }

        [ClientRpc]
        public void AffectClientRpc(ulong targetID, ClientRpcParams rpcParams = default)
        {
            if (IsClient)
            {
                IHp targetHp = InGameManager.instance.player[targetID].GetComponent<IHp>();
                Debug.Log($"target Hp Before {targetHp.HpValue}");
                targetHp.RecoverHp(amount);
                Debug.Log($"target Hp After {targetHp.HpValue}");
                gameObject.SetActive(false);
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
            gameObject.SetActive(false);
        }
    }
}