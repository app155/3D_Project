using Project3D.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDebuff
{
    public float duration { get; set; }

    public void Debuff();
}
