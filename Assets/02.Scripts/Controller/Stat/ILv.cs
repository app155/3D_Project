using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public interface ILv
    {
        public int LvValue { get; set; }
        public int LvMax { get; set; }
        public int LvMin { get; }

        public event Action<int> onLvChanged;
        public event Action onLvMax;
        public event Action onLvMin;

        public void LvUp(int amount);

       
    }
}
