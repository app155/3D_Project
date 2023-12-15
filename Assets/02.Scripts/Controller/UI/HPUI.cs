using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterControllers = Project3D.Controller.CharacterControllers;
using Project3D.Controller;
using UnityEngine.UI;
using Unity.Netcode;
using System;

namespace Project3D.UI
{
    public class HPUI : NetworkBehaviour
    {
        [SerializeField] private Slider _hpBar;
        [SerializeField] private Slider _Lv;
        private void Start()
        {
            IHp hp = transform.root.GetComponent<IHp>();
            _hpBar.minValue = hp.HpMin;
            _hpBar.maxValue = hp.HpMax;
            _hpBar.value = hp.HpValue;
            _Lv.value = hp.Lv;

            hp.onLvChanged += (value) => _Lv.value = value;
            hp.onHpChanged += (value) => _hpBar.value = value;

            // is Ű���� 
            // ��ü�� � Ÿ������ ������ �� �ִ��� Ȯ���ϰ� bool ����� ��ȯ�ϴ� Ű����
            if (hp is CharacterController)
            {
                Vector3 originScale = transform.localScale;
                ((CharacterController)hp).onDirectionChanged += (value) =>
                {
                    transform.localScale = value < 0 ?
                        new Vector3(-originScale.x, originScale.y, originScale.z) :
                        new Vector3(+originScale.x, originScale.y, originScale.z);
                };
            }
        }
     
    }
}

// is as ����

