using Project3D.GameSystem;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using System.Collections;

namespace Project3D.UI
{
    public class UI_TopBoard : UIMonobehaviour
    {
        [SerializeField] private TMP_Text _blueTeamScore;
        [SerializeField] private TMP_Text _redTeamScore;

        private void Start()
        {
            StartCoroutine(C_Init());
        }
        IEnumerator C_Init()
        {
            yield return new WaitUntil(() => InGameManager.instance != null);
            InGameManager.instance.onScoreState += RefreshServerRpc;
        }
        public override void InputAction()
        {
            
        }

        public void Refresh()
        {
            _blueTeamScore.text = InGameManager.instance.blueTeam.score.ToString();
            _redTeamScore.text = InGameManager.instance.redTeam.score.ToString();
            Debug.Log($"blue = {InGameManager.instance.blueTeam.score}, red = {InGameManager.instance.redTeam.score}");
        }

        [ServerRpc(RequireOwnership = false)]
        public void RefreshServerRpc()
        {
            RefreshClientRpc();
        }

        [ClientRpc]
        public void RefreshClientRpc()
        {
            _blueTeamScore.text = InGameManager.instance.blueTeam.score.ToString();
            _redTeamScore.text = InGameManager.instance.redTeam.score.ToString();
            Debug.Log($"blue = {InGameManager.instance.blueTeam.score}, red = {InGameManager.instance.redTeam.score}");
        }
    }
}