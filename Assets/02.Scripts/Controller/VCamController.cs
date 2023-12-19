using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Project3D.Controller
{
    public class VCamController : MonoBehaviour
    {
        [SerializeField] private GameObject _target;
        [SerializeField] private Vector3 _ballViewOffset;
        [SerializeField] private Vector3 _scorerViewOffset;

        private CinemachineVirtualCamera _vCam;

        private void Awake()
        {
            _vCam = GetComponent<CinemachineVirtualCamera>();
        }

        void Start()
        {

        }

        void Update()
        {

        }

        [ServerRpc(RequireOwnership = false)]
        public void ShowScorerServerRpc()
        {

        }

        [ClientRpc]
        public void ShowScorerClientRpc()
        {

        }
    }
}
