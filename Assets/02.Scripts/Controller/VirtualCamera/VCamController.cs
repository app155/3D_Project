using Cinemachine;
using Project3D.GameSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

namespace Project3D.Controller
{
    public class VCamController : NetworkBehaviour
    {
        [SerializeField] private PlayableDirector _inGameDirector;
        [SerializeField] private PlayableDirector _afterGameDirector;

        [SerializeField] private CinemachineVirtualCamera _ballFollowCam;
        [SerializeField] private CinemachineVirtualCamera _scorerZoomCam;
        [SerializeField] private CinemachineVirtualCamera _winningCam;

        private void Awake()
        {
            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _ballFollowCam.Priority = 11;
            _scorerZoomCam.Priority = 10;
            _winningCam.Priority = 1;

            InGameManager.instance.onStandbyState += ReturnToBallClientRpc;
            InGameManager.instance.onScoreState += ShowScorerClientRpc;
            InGameManager.instance.onEndState += ShowWinnerClientRpc;
        }


        [ServerRpc(RequireOwnership = false)]
        public void ReturnToBallServerRpc()
        {
            ReturnToBallClientRpc();
        }

        [ClientRpc]
        public void ReturnToBallClientRpc()
        {
            _scorerZoomCam.Priority = 10;
            _ballFollowCam.Priority = 11;

            Vector3 ballCamPos = _ballFollowCam.transform.position;
            _ballFollowCam.ForceCameraPosition(new Vector3(0.0f, ballCamPos.y, ballCamPos.z),
                                               _ballFollowCam.transform.rotation);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ShowScorerServerRpc()
        {
            ShowScorerClientRpc();
        }

        [ClientRpc]
        public void ShowScorerClientRpc()
        {
            if (InGameManager.instance.player.ContainsKey(InGameManager.instance.scorerID))
            {
                _scorerZoomCam.Priority = 11;
                _ballFollowCam.Priority = 10;
                _scorerZoomCam.ForceCameraPosition(_ballFollowCam.transform.position, Quaternion.Euler(new Vector3(40.0f, 0.0f, 0.0f)));
                _scorerZoomCam.Follow = InGameManager.instance.player[InGameManager.instance.scorerID].transform;
            }

            _inGameDirector.Play();
            _ballFollowCam.transform.position = new Vector3(0.0f, _ballFollowCam.transform.position.y, _ballFollowCam.transform.position.z);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ShowWinnerServerRpc()
        {
            ShowWinnerClientRpc(3);
        }

        [ClientRpc]
        public void ShowWinnerClientRpc(int dummy)
        {
            _winningCam.Priority = 12;

            _afterGameDirector.Play();
        }
    }
}
