using System;
using UnityEngine;

namespace Project3D.GameElements.Skill
{
    public abstract class Skill : MonoBehaviour
    {
        public string description;

        protected float coolTimer;
        protected float coolTime;
        protected float castTimer;
        protected float castTime;

        public abstract void Execute();
    }
}