using Unity.Netcode;
using UnityEngine;

namespace Project3D.GameElements.Items
{
    public interface IItem
    {
        [ServerRpc(RequireOwnership = false)]
        public void AffectServerRpc(ServerRpcParams rpcParams = default);
    }
}