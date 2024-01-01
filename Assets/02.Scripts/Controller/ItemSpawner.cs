using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;
using Project3D.GameSystem;

namespace Project3D.Controller
{
    public class ItemSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject[] _itemPrefabs;
        private BoxCollider _col;

        private float _maxX;
        private float _minX;
        private float _maxZ;
        private float _minZ;

        [SerializeField] private float _minTime = .1f;
        [SerializeField] private float _maxTime = .2f;

        private float waitTime;
        private float startTime;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _col = GetComponent<BoxCollider>();

            _maxX = _col.size.x / 2.0f;
            _minX = -_col.size.x / 2.0f;
            _maxZ = _col.size.z / 2.0f;
            _minZ = -_col.size.z / 2.0f;

            Setup();

            InGameManager.instance.onStandbyState += Setup;
        }

        private void Update()
        {
            if (IsServer == false)
                return;

            if (InGameManager.instance.gameState == GameState.Score)
                return;

            if (_col == null)
                return;

            if (waitTime > Time.time - startTime)
                return;

            SpawnItem(0);
            Setup();
        }

        private void Setup()
        {
            waitTime = GetNextTime();
            startTime = Time.time;
        }

        public void SpawnItem(int itemID)
        {
            float randomX = Random.Range(_minX, _maxX);
            float randomZ = Random.Range(_minZ, _maxZ);

            Vector3 spawnPos = new Vector3(randomX, 0.0f, randomZ);


            GameObject go = Instantiate(_itemPrefabs[itemID], spawnPos, Quaternion.identity);

            go.GetComponent<NetworkObject>().Spawn();
        }

        private float GetNextTime()
        {
            return Random.Range(_minTime, _maxTime);
        }
    }
}
