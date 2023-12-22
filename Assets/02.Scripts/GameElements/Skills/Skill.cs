using Project3D.Controller;
using System;
using Unity.Netcode;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;

namespace Project3D.GameElements.Skill
{
    public abstract class Skill : NetworkBehaviour
    {
        public string description;

        protected CharacterController owner;
        [SerializeField] protected float coolTime;

        public virtual void Init(CharacterController owner)
        {
            this.owner = owner;
        }

        public abstract void Execute();
        public abstract void Casting();
    }
}