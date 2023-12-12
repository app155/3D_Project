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

        public event Action<float> onHpChanged; // float : �ٲ� ���� ü��
        public event Action<float> onHpRecovered; // float : ȸ���� ��
        public event Action<float> onHpDepleted; // float : ���� ��
        public event Action onHpMax;
        public event Action onHpMin;

        public void DepleteHp(float amount);
        public void RecoverHp(float amount);
    }
}