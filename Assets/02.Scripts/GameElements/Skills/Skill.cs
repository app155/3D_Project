using Project3D.Controller;
using System;
using Unity.Netcode;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterController;

namespace Project3D.GameElements.Skill
{
    public abstract class Skill : NetworkBehaviour
    {
        public string description;

        protected CharacterController owner;
        protected float coolTimer;
        protected float coolTime;
        protected float castTimer;
        protected float castTime;

        public virtual void Init(CharacterController owner)
        {
            this.owner = owner;
        }

        public abstract void Execute();
    }
}