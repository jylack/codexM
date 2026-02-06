using System.Collections.Generic;
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "_Project/Maps/MapData", fileName = "MapData")]
    public class MapData : ScriptableObject
    {
        public List<StageDefinitionSO> stages = new List<StageDefinitionSO>();
    }
}
