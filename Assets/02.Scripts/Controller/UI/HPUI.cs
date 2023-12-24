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
        [SerializeField] private Slider _Lv;
        [SerializeField] private Image _hpBarBackground;

        private void Start()
        {
            IHp hp = transform.root.GetComponent<IHp>();
            _hpBar.minValue = hp.hpMin;
            _hpBar.maxValue = hp.hpMax;
            _hpBar.value = hp.hpValue;
            //_Lv.value = hp.Lv;

            hp.onHpChanged += (value) => _hpBar.value = value;
            //hp.onLvChanged += (value) => _Lv.value = value;

            // is 키워드 
            // 객체가 어떤 타입으로 참조할 수 있는지 확인하고 bool 결과를 반환하는 키워드
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

        private void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
        }
    }
}

// is as 예시

