using Project3D.GameSystem;
using Project3D.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Project3D.UI
{
    public class UI_GameResult : UIMonobehaviour
    {
        [SerializeField] TMP_Text _winnerTeamText;


        // ToModify
        public override void Init()
        {
            base.Init();
            InGameManager.instance.onEndState += ShowWinnerTeamTextClientRpc;
        }

        [ClientRpc]
        private void ShowWinnerTeamTextClientRpc(int value)
        {
            _winnerTeamText.color = value == 0 ? Color.blue : Color.red;
            Show();
        }

        public override void InputAction()
        {

        }
    }
}
