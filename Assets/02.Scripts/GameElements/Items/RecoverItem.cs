using Project3D.Controller;
using Unity.Netcode;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterController;

namespace Project3D.GameElements.Items
{
    public class RecoverItem : Item, IItem
    {
        [SerializeField] float amount;

        public override void Affect(int targetID)
        {
            AffectServerRpc(targetID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void AffectServerRpc(int targetID, ServerRpcParams rpcParams = default)
        {
            CharacterController targetChara = NetworkManager.Singleton.ConnectedClientsList[targetID].PlayerObject.GetComponent<CharacterController>();
            IHp target = targetChara.GetComponent<IHp>();
            Debug.Log($"target HP Before {target.HpValue}");
            target.RecoverHp(amount);
            gameObject.SetActive(false);
            Debug.Log($"target Hp After {target.HpValue}");
        }

        [ClientRpc]
        public void AffectClientRpc(int targetID, ClientRpcParams rpcParams = default)
        {
            CharacterController targetChara = NetworkManager.Singleton.ConnectedClientsList[targetID].PlayerObject.GetComponent<CharacterController>();
            IHp target = targetChara.GetComponent<IHp>();
            Debug.Log($"target HP Before {target.HpValue}");
            target.RecoverHp(amount);
            gameObject.SetActive(false);
            Debug.Log($"target Hp After {target.HpValue}");
        }
    }
}