using System;
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

        public event Action<float> onHpChanged; // float : 바뀐 이후 체력
        public event Action<float> onHpRecovered; // float : 회복된 양
        public event Action<float> onHpDepleted; // float : 깎인 양
        public event Action onHpMax;
        public event Action onHpMin;

        public void DepleteHp(float amount);
        public void RecoverHp(float amount);
    }
}