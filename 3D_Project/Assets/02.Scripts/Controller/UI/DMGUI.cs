using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterControllers;
using Project3D.Controller;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using DG.Tweening;
using TMPro;

namespace Project3D.UI.DMGUI
{
    public class DMGUI : MonoBehaviour
    {
        private TMP_Text _amount;
        private float _fadeSpeed = 0.8f;
        private Vector3 _move = new Vector3(0.0f, 0.3f, 0.0f);
        private Color _color;

        public void Show(float amount)
        {
            _amount.text = ((int)amount).ToString();
            _color.a = 1.0f;
            _amount.color = _color;
            Debug.Log("출력완료");
        }

        private void Awake()
        {
            _amount = GetComponent<TMP_Text>();
            _color = _amount.color;
        }

        private void Update()
        {
            transform.position += _move * Time.deltaTime;
            _color.a -= _fadeSpeed * Time.deltaTime;
            _amount.color = _color;

            if (_color.a <= 0.0f)
            {
                gameObject.SetActive(false);
            }
        }


    }
}
