using Project3D.GameSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TestUI_CountDown : NetworkBehaviour
{
    [SerializeField] private TMP_Text _countdownText;

    // Start is called before the first frame update
    void Start()
    {
        InGameManager.instance.onCountdownChanged += (value) => RefreshServerRpc(value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RefreshServerRpc(float value)
    {
        RefreshClientRpc(value);
    }

    [ClientRpc]
    public void RefreshClientRpc(float value)
    {
        _countdownText.text = value.ToString();
    }
}
