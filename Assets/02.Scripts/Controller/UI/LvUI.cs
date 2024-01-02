using System.Collections;
using CharacterController = Project3D.Controller.CharacterControllers;
using System.Collections.Generic;
using UnityEngine;
using Project3D.Controller;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using TMPro;

namespace Project3D.UI
{
    public class LvUI : NetworkBehaviour
    {
        [SerializeField] public TMP_Text _Lv;
        [SerializeField] private Image _lvCircleBackground;

        private void Start()
        {
            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ILv Lv = transform.root.GetComponent<ILv>();
            _Lv.text = "1";

            Lv.onLvChanged += (value) => _Lv.text = value.ToString();
        }

        private void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward);
        }
    }
}


