using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ExpBar : NetworkBehaviour
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
            if (_expValue.Value <= 100)
            {
                expSlider.value = (float)_expValue.Value / 100;
                Debug.Log(_expValue.Value);
                return;
            }

            _LvValue.Value += _expValue.Value / 100;
            _expValue.Value = _expValue.Value % 100;

            // OnValueChanged 이벤트를 수동으로 호출하여 UI 업데이트
            _expValue.OnValueChanged?.Invoke(_expValue.Value - current, _expValue.Value);
        };

        _LvValue.OnValueChanged += (prev, current) =>
        {
            if (prev != current)
                LvText.text = _LvValue.Value.ToString();
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncreaseExpServerRpc(int amount)
    {
        _expValue.Value += amount;
    }

    // 클라이언트에서 호출하는 함수
    public void IncreaseExp(int amount)
    {
            IncreaseExpServerRpc(amount);
    }
}
