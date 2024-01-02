using System.Collections;
using CharacterController = Project3D.Controller.CharacterControllers;
using System.Collections.Generic;
using UnityEngine;
using Project3D.Controller;
using UnityEngine.UI;
using Unity.Netcode;
using System;

namespace Project3D.UI
{
    public class HPUI : NetworkBehaviour
    {
        [SerializeField] private Slider _hpBar;
        [SerializeField] private Image _hpBarBackground;

        private void Start()
        {
            
            //hp.onLvChanged += (value) => _Lv.value = value;

            // is Ű���� 
            // ��ü�� � Ÿ������ ������ �� �ִ��� Ȯ���ϰ� bool ����� ��ȯ�ϴ� Ű����
            //if (hp is CharacterControllers)
            //{
            //    Vector3 originScale = transform.localScale;
            //    ((CharacterControllers)hp).onDirectionChanged += (value) =>
            //    {
            //        transform.localScale = value < 0 ?
            //            new Vector3(-originScale.x, originScale.y, originScale.z) :
            //            new Vector3(+originScale.x, originScale.y, originScale.z);
            //    };
            //}
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            IHp hp = transform.root.GetComponent<IHp>();
            _hpBar.minValue = hp.hpMin;
            _hpBar.maxValue = hp.hpMax;
            _hpBar.value = hp.hpValue;


            hp.onHpChanged += (value) => _hpBar.value = value;
        }

        private void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
        }
    }
}

// is as ����

