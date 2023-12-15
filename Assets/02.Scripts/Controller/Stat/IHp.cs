using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project3D.Controller
{
    public interface IHp : IKnockback
    {
        public float HpValue { get; set; }
        public float HpMax { get; set; }
        public float HpMin { get; }
        public int Lv { get; set; }
        public event Action<int> onLvChanged;
        public event Action<float> onHpChanged; 
        public event Action<float> onHpRecovered; 
        public event Action<float> onHpDepleted;
        public event Action onHpMax;
        public event Action onHpMin;


        public void DepleteHp(float amount);
        public void RecoverHp(float amount);
    }
}