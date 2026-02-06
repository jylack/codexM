// (확장용) 맵 단위에서 스테이지 목록을 보관하는 ScriptableObject입니다.
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
