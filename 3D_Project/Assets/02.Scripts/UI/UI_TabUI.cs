using Project3D.Controller;
using Project3D.GameSystem;
using Project3D.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_TabUI : UIMonobehaviour
{
    [SerializeField] private TMP_Text[] _playersInfoText;

    public override void Init()
    {
        base.Init();

        // temp
    }

    public override void InputAction()
    {
        
    }

    private void Start()
    {
        InGameManager.instance.onScoreState += Refresh;
    }

    public void Refresh()
    {
        for (int i = 0; i < InGameManager.instance.player.Count; i++)
        {
            _playersInfoText[i].text =
                $"{InGameManager.instance.player[(ulong)i].GetComponent<CharacterControllers>().score}";
        }

        // RefreshServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RefreshServerRpc()
    {
        for (int i = 0; i < InGameManager.instance.player.Count; i++)
        {
            _playersInfoText[i].text =
                $"{InGameManager.instance.player[(ulong)i].GetComponent<CharacterControllers>().score}";
        }

        // RefreshClientRpc();
    }

    [ClientRpc]
    public void RefreshClientRpc()
    {
        for (int i = 0; i < InGameManager.instance.player.Count; i++)
        {
            _playersInfoText[i].text =
                $"{InGameManager.instance.player[(ulong)i].GetComponent<CharacterControllers>().score}";
        }
    }
}
