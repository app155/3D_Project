using Project3D.Controller;
using Project3D.GameSystem;
using Project3D.Lobbies;
using Project3D.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UI_TabUI : UIMonobehaviour
{
    [SerializeField] private TMP_Text[] _blueInfoText;
    [SerializeField] private TMP_Text[] _redInfoText;

    public override void Init()
    {
        base.Init();
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
        List<ulong> blueTeam = InGameManager.instance.blueTeam.GetPlayersInTeam();
        List<ulong> redTeam = InGameManager.instance.redTeam.GetPlayersInTeam();

        for (int i = 0; i < blueTeam.Count; i++)
        {
            _blueInfoText[i].text =
                $"{GameLobbyManager.instance.lobbyPlayerDatas[(int)blueTeam[i]].NickName}\n{InGameManager.instance.player[blueTeam[i]].GetComponent<CharacterControllers>().score} Goal";
        }

        for (int i = 0; i < redTeam.Count; i++)
        {
            _redInfoText[i].text =
                $"{GameLobbyManager.instance.lobbyPlayerDatas[(int)redTeam[i]].NickName}\n{InGameManager.instance.player[redTeam[i]].GetComponent<CharacterControllers>().score} Goal";
        }

        // RefreshServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RefreshServerRpc()
    {
        List<ulong> blueTeam = InGameManager.instance.blueTeam.GetPlayersInTeam();
        List<ulong> redTeam = InGameManager.instance.redTeam.GetPlayersInTeam();

        for (int i = 0; i < blueTeam.Count; i++)
        {
            _blueInfoText[i].text =
                $"{InGameManager.instance.player[blueTeam[i]].GetComponent<CharacterControllers>().score}";
        }

        for (int i = 0; i < redTeam.Count; i++)
        {
            _redInfoText[i].text =
                $"{InGameManager.instance.player[redTeam[i]].GetComponent<CharacterControllers>().score}";
        }

        // RefreshClientRpc();
    }

    [ClientRpc]
    public void RefreshClientRpc()
    {
        List<ulong> blueTeam = InGameManager.instance.blueTeam.GetPlayersInTeam();
        List<ulong> redTeam = InGameManager.instance.redTeam.GetPlayersInTeam();

        for (int i = 0; i < blueTeam.Count; i++)
        {
            _blueInfoText[i].text =
                $"{InGameManager.instance.player[blueTeam[i]].GetComponent<CharacterControllers>().score}";
        }

        for (int i = 0; i < redTeam.Count; i++)
        {
            _redInfoText[i].text =
                $"{InGameManager.instance.player[redTeam[i]].GetComponent<CharacterControllers>().score}";
        }
    }
}
