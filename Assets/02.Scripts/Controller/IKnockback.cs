using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public interface IKnockback
    {
        [ServerRpc(RequireOwnership = false)]
        public void KnockbackServerRpc(Vector3 pushDir, float pushPower);
    }
}