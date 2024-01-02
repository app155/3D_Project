using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    public Slider expSlider;
    public TMP_Text LvText;
    Action OnExpUp;
    Action OnLvUp;
    [SerializeField] private int _expValue;
    [SerializeField] private int _LvValue;

    [ClientRpc]
    public void ExpValueClientRpc(int exp)
    {
        expValue = exp;
    }

    private void Awake()
    {
        _expValue = 0;
        _LvValue = 1;
        
    }
    public int expValue
    {
        get 
        {
            return _expValue;
        }
        set 
        {
            _expValue += value;
            if (_expValue <= 100)
            {
                expSlider.value = (float)_expValue / 100;
                return;
            }
            LvValue += _expValue/100;
            _expValue = _expValue % 100;
            expSlider.value = (float)_expValue/100;
        }
    }

    public int LvValue
    {
        get 
        { 
            return _LvValue; 
        } 
        set 
        {

            _LvValue = value; 
            LvText.text = _LvValue.ToString();
        }
    }

}
