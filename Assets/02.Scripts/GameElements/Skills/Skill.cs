using Project3D.Controller;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.GameElements.Skill
{
    public abstract class Skill : NetworkBehaviour
    {
        public string description;

        protected CharacterControllers owner;
        [SerializeField] protected float castTimer;
        [SerializeField] protected float castTime;
        protected ulong clientID;


        public virtual void Init(CharacterControllers owner)
        {
            this.owner = owner;
            clientID = owner.GetComponent<NetworkBehaviour>().OwnerClientId;
        }

        public abstract void Execute();
        public abstract void Casting();
    }
}