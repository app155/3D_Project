
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project3D.Lobbies
{
    [CreateAssetMenu(menuName = "Data/MapSelectionData", fileName = "MapSelectionData")]
    public class MapSelectionData : ScriptableObject
    {

        public List<MapInfo> maps;


    }
}

[Serializable]
public struct MapInfo
{
    public Color mapThumnail; // ∏  º±≈√ 
    public string mapName;
    public string sceneName;

}