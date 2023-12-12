using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project3D.Controller
{
    public interface IHp
    {
        public float HpValue { get; set; }
        public float HpMax { get; set; }
        public float HpMin { get; }

        public void DepleteHp(float amount);
        public void RecoverHp(float amount);
    }
}