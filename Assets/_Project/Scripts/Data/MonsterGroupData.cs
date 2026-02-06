// 몬스터 그룹 ScriptableObject 스키마 정의입니다.
using System.Collections.Generic;
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "_Project/Monsters/MonsterGroupData", fileName = "MonsterGroup_")]
    public class MonsterGroupData : ScriptableObject
    {
        public string id;
        public bool isBossGroup;
        public List<MonsterData> monsters = new List<MonsterData>();
    }
}
