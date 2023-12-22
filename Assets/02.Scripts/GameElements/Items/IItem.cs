using Unity.Netcode;
using UnityEngine;

namespace Project3D.GameElements.Items
{
    public interface IItem
    {
        [ServerRpc(RequireOwnership = false)]
        public void AffectServerRpc(ulong clientID, ServerRpcParams rpcParams = default);

        [ServerRpc(RequireOwnership = false)]
        public void DisappearServerRpc(ServerRpcParams rpcParams = default);
    }
}