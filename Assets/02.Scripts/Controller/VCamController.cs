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
        [SerializeField] PlayableDirector _director;

        private CinemachineVirtualCamera[] _vCams;
        private CinemachineVirtualCamera _ballFollowCam;
        private CinemachineVirtualCamera _scorerZoomCam;

        private void Awake()
        {
            
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _director = GetComponent<PlayableDirector>();
            _vCams = GetComponentsInChildren<CinemachineVirtualCamera>();

            _ballFollowCam = _vCams[0];
            _scorerZoomCam = _vCams[1];

            InGameManager.instance.onStandbyState += ReturnToBallClientRpc;
            InGameManager.instance.onScoreState += ShowScorerClientRpc;
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
        }

        [ServerRpc(RequireOwnership = false)]
        public void ShowScorerServerRpc()
        {
            ShowScorerClientRpc();
        }

        [ClientRpc]
        public void ShowScorerClientRpc()
        {
            _scorerZoomCam.Priority = 11;
            _ballFollowCam.Priority = 10;
            _scorerZoomCam.Follow = InGameManager.instance.player[InGameManager.instance.scorerID].transform;
            _director.Play();
        }
    }
}
