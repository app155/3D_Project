using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public static ExpBar _instance;
    private void Awake()
    {

        if (_instance == null)
        {
            _instance = this;
        }

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
