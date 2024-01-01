using Project3D.GameSystem;
using Project3D.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TestUI_CountDown : UIMonobehaviour
{
    [SerializeField] private TMP_Text _countdownText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        InGameManager.instance.onCountdownChanged += (value) => RefreshServerRpc(value);
    }

    public override void InputAction()
    {

    }

    [ServerRpc(RequireOwnership = false)]
    public void RefreshServerRpc(float value)
    {
        RefreshClientRpc(value);
    }

    [ClientRpc]
    public void RefreshClientRpc(float value)
    {
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

    
}
