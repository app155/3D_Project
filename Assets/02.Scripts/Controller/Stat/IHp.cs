using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public interface IHp : IKnockback
    {
        public float hpValue { get; set; }
        public float hpMax { get; set; }
        public float hpMin { get; }
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