using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ExpBar : MonoBehaviour
{
    public Slider expSlider;
    public TMP_Text LvText;
    public NetworkVariable<int> _expValue;
    public NetworkVariable<int> _LvValue;


    private void Awake()
    {
        _expValue = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
        _LvValue = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        _expValue.OnValueChanged += (prev, current) =>
        {
            _expValue.Value += current;
            if (_expValue.Value <= 100)
            {
                expSlider.value = (float)_expValue.Value/100;
                Debug.Log(_expValue.Value);
                return;
            }
            _LvValue.Value += _expValue.Value / 100;
            _expValue.Value = _expValue.Value % 100;
            expSlider.value = (float)_expValue.Value / 100;
            Debug.Log(_expValue.Value);
        };

        _LvValue.OnValueChanged += (prev, current) =>
        {
            if(prev != current)
            LvText.text = _LvValue.Value.ToString();
        };
    }

    /*  public int expValue
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
    */
}
