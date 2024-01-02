using DG.Tweening.Core.Easing;
using Project3D.GameSystem;
using Project3D.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TestUI_CountDown : MonoBehaviour, IUI
{
    public static TestUI_CountDown instance;

    [SerializeField] private TMP_Text _countdownText;
    bool _hasInitailized;
    bool _hasStarted;

    public event Action onShow;
    public event Action onHide;

    public int sortingOrder
    {
        get => instance.sortingOrder;
        set => instance.sortingOrder = value;
    }

    public bool inputActionEnabled { get; set; }

    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _hasStarted = true;
        InGameManager.instance.onCountdownChanged += (value) => Refresh(value);
        _hasInitailized = true;
    }

    public void Refresh(float value)
    {
        Debug.Log("123123124141231231242314241324");

        if (_hasInitailized == false ||
            _hasStarted == false)
        {
            return;
        }

        if (value <= -1f)
        {
            _countdownText.text = string.Empty;
            Hide();
        }

        else if (value >= 5.0f)
        {
            Show();
        }

        else
        {
            _countdownText.text = value.ToString();
        }
    }

    public void InputAction()
    {

    }

    public void Show()
    {
        gameObject.SetActive(true);
        onShow?.Invoke();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        onHide?.Invoke();
    }

    //public void RefreshServerRpc(float value)
    //{
    //    RefreshClientRpc(value);
    //}
    //public void RefreshClientRpc(float value)
    //{
    //    if (_hasInitailized == false ||
    //        _hasStarted == false)
    //    {
    //        return;
    //    }

    //    if (value <= -1f)
    //    {
    //        _countdownText.text = string.Empty;
    //        Hide();
    //    }

    //    else if (value >= 5.0f)
    //    {
    //        Show();
    //    }

    //    else
    //    {
    //        _countdownText.text = value.ToString();
    //    }
    //}


}
