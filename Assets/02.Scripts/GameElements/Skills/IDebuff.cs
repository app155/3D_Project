using Project3D.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project3D.GameElements.Skill
{
    public interface IDebuff
    {
        public float duration { get; set; }

        public void Debuff();
    }
}