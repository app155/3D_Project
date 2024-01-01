using System.Collections.Generic;
using UnityEngine;

namespace Project3D.GameSystem
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _initializables = new List<GameObject>();

        private void Start()
        {
            foreach (GameObject go in _initializables)
            {
                if (go.TryGetComponent(out IInitializable target))
                {
                    target.Init();
                }
            }
        }
    }
}