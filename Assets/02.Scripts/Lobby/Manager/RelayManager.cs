using Unity.VisualScripting;
using System;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using System.Linq;
using UnityEngine;
using System.Security.Cryptography;

namespace Project3D.Lobbies.Manager
{
    public class RelayManager : MonoBehaviour
    {
        public static RelayManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("RelayManager");
                    _instance = go.AddComponent<RelayManager>();
                    DontDestroyOnLoad(go);


                }
                return _instance;
            }
        }

        public bool IsHost
        {
            get
            {
                return _isHost;
            }

        }

        private bool _isHost = false;
        private static RelayManager _instance;
        private string _joinCode;
        private string _ip;
        private int _port;
        private byte[] _key;
        private byte[] _connectionData;
        private byte[] _hostConnectionData;


        private System.Guid _allocationId;
        private byte[] _allocationIdBytes;

        public string GetAllocationId()
        {
            return _allocationId.ToString();
        }

        public string GetConnectionData()
        {
            return _connectionData.ToString();
        }

        public async Task<string> CreateRelay(int maxConnection)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerEndpoint dtlsEnpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            _ip = dtlsEnpoint.Host;
            _port = dtlsEnpoint.Port;

            _allocationId = allocation.AllocationId;
            _allocationIdBytes = allocation.AllocationIdBytes;
            _connectionData = allocation.ConnectionData;
            _key = allocation.Key;

            _isHost = true;

            return _joinCode;
        }
        public async Task<bool> JoinRelay(string joinCode)
        {
            _joinCode = joinCode;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerEndpoint dtlsEnpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
            _ip = dtlsEnpoint.Host;
            _port = dtlsEnpoint.Port;

            _allocationId = allocation.AllocationId;
            _allocationIdBytes = allocation.AllocationIdBytes;
            _key = allocation.Key;
            _hostConnectionData = allocation.HostConnectionData;
            _connectionData = allocation.ConnectionData;

            return true;
        }

        public (byte[] AllocationId, byte[] key, byte[] ConnectionData, string _dtlsAddress, int _dtlsPort) GetHostConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _ip, _port);
        }
        public (byte[] AllocationId, byte[] key, byte[] ConnectionData, byte[] HostConnectionData, string _dtlsAddress, int _dtlsPort) GetClientConnectionInfo()
        {
            return (_allocationIdBytes, _key, _connectionData, _hostConnectionData, _ip, _port);
        }
    }
}