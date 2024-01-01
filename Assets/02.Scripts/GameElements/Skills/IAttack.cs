using Project3D.Controller;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.GameElements.Skill
{
    public interface IAttack
    {
        public void Attack(ulong targetID);
    }
}