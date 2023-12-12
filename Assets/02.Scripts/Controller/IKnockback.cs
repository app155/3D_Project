using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project3D.Controller
{
    public interface IKnockback
    {
        public void Knockback(Vector3 pushDir, float pushPower);
    }
}